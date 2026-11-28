using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;
using SubApp1.Models;
using SubApp1.ViewModels;

namespace SubApp1.Services;

public class FurnitureService : IFurnitureService
{
    private readonly QuizDbContext _db;
    private readonly ILogger<FurnitureService> _logger;

    public FurnitureService(QuizDbContext db, ILogger<FurnitureService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Counts every attempt, so retaking a quiz also counts. Easier to test; change back to Distinct() later.
    private Task<int> CountCompletedQuizzesAsync(int userId) =>
        _db.QuizAttempts.CountAsync(a => a.UserId == userId);
        
    // Count number of distinct quizzes the student has attempted (retaking the same quiz does not unlock anything).
    //private Task<int> CountCompletedQuizzesAsync(int userId) =>
    //    _db.QuizAttempts
    //        .Where(a => a.UserId == userId)
    //        .Select(a => a.QuizId)
    //        .Distinct()
    //        .CountAsync();

    public async Task<UnlockResult> CheckUnlockAsync(int userId)
    {
        var result = new UnlockResult();
        try
        {
            var completed = await CountCompletedQuizzesAsync(userId);
            var totalAttempts = await _db.QuizAttempts.CountAsync(a => a.UserId == userId);
            result.CasJustFreed = totalAttempts == 1;

            var owned = await _db.StudentFurnitures.CountAsync(sf => sf.UserId == userId);
            if (!UnlockRules.ShouldUnlockFurniture(completed, owned))
            {
                return result;
            }

            // First furniture (by SortOrder) that this student does not own yet
            var next = await _db.FurnitureItems
                .Where(f => !_db.StudentFurnitures.Any(sf => sf.UserId == userId && sf.FurnitureId == f.Id))
                .OrderBy(f => f.SortOrder)
                .FirstOrDefaultAsync();

            if (next is null)
            {
                return result; // everything is already unlocked 
            }


            _db.StudentFurnitures.Add(new StudentFurniture
            {
                UserId = userId,
                FurnitureId = next.Id,
                UnlockedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            result.NewFurniture = next;
            _logger.LogInformation("[FurnitureService] Student {UserId} unlocked {Slug}", userId, next.Slug);
        }   
        catch (Exception ex)
        {
            // A failed unlock must never break grading
            _logger.LogError(ex, "[FurnitureService] Unlock check failed for student {UserId}", userId);
        }
        return result;
    }

    public async Task<SceneViewModel> GetSceneAsync(int userId)
    {
        try
        {
            var completed = await CountCompletedQuizzesAsync(userId);
            var slugs = await _db.StudentFurnitures
                .AsNoTracking()
                .Where(sf => sf.UserId == userId)
                .OrderBy(sf => sf.Furniture.SortOrder)
                .Select(sf => sf.Furniture.Slug)
                .ToListAsync();

            return new SceneViewModel
            {
                CatOutOfBox = UnlockRules.CatIsFree(completed),
                FurnitureSlugs = slugs
            };
        }
        catch (Exception ex)
        {
            _logger .LogError(ex, "[FurnitureService] Could not load the scene for the student {UserId}", userId);
            return new SceneViewModel(); // empty room, but the site still works
        }
    }
}