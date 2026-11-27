using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

// Seed test data for development environment 
public static class DBInit
{
    public static async Task SeedAsync(IApplicationBuilder app)
    {
        await using var serviceScope = app.ApplicationServices.CreateAsyncScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<QuizDbContext>();

        await context.Database.MigrateAsync();

        // Everything is seeded together, so if Courses exist -> the seed has already run
        if (await context.Courses.AnyAsync()) return;

        // create courses
        var testCourse = new Course { Name = "Test course", Code = "TEST1001" };
        var webCourse = new Course { Name = "Web applications", Code = "ITPE3200" };

        var alice = new User
        {
            Name = "Alice Hansen",
            UserName = "alice@hansen.no",
            NormalizedUserName = "ALICE@HANSEN.NO",
            Email = "alice@hansen.no",
            NormalizedEmail = "ALICE@HANSEN.NO"
        };
        var bob = new User
        {
            Name = "Bob Johansen",
            UserName = "bob@johansen.no",
            NormalizedUserName = "BOB@JOHANSEN.NO",
            Email = "bob@johansen.no",
            NormalizedEmail = "BOB@JOHANSEN.NO"
        };

        // create quiz with questions
        context.Quizzes.AddRange(
            new Quiz
            {
                Title = "Test",
                Description = "A test quiz",
                Course = testCourse,
                CreatedByUser = alice,
                Questions = new List<QuizQuestion>
                {
                    // questions
                    new() { Prompt = "A", CorrectAnswer = "A", Points = 1, Order = 1 },
                    new() { Prompt = "B", CorrectAnswer = "B", Points = 1, Order = 2 },
                    new() { Prompt = "123", CorrectAnswer = "123", Points = 1, Order = 3 }
                }
            },
            new Quiz
            {
                Title = "Web basics Quiz",
                Description = "A short introduction to web development.",
                Course = webCourse,
                CreatedByUser = bob,
                Questions = new List<QuizQuestion>
                {
                    new() { Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 },
                    new() { Prompt = "What does HTML stand for?", CorrectAnswer = "Hypertext Markup Language", Points = 1, Order = 2 },
                    new() { Prompt = "Which language is commonly used to add interactivity to webpages?", CorrectAnswer = "JavaScript", Points = 1, Order = 3 },
                    new() { Prompt = "What does CSS stand for?", CorrectAnswer = "Cascading Style Sheets", Points = 1, Order = 4 },
                    new() { Prompt = "Which HTTP method is commonly used to retrieve data?", CorrectAnswer = "GET", Points = 1, Order = 5 },
                    new() { Prompt = "Which HTTP method is commonly used to submit data?", CorrectAnswer = "POST", Points = 1, Order = 6 }
                }
            });

        // EF inserts users and courses through the quiz relationships,
        // then the quizzes, then the questions, in the right order, in a single transaction -> avoids half seeded db
        await context.SaveChangesAsync();
    }
}