using SubApp1.Models;
using SubApp1.ViewModels;

namespace SubApp1.Services;

public class UnlockResult
{
    public bool CasJustFreed { get; set; }
    public Furniture? NewFurniture { get; set; }
}

public interface IFurnitureService
{
    // Called after a quiz is attempt is saved. Returns what (if anything) was unlocked. 
    Task<UnlockResult> CheckUnlockAsync(int studentId);

    // What the room should look like for this student
    Task<SceneViewModel> GetSceneAsync(int studentId);
}