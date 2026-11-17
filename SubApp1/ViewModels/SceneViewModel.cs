namespace SubApp1.ViewModels;

// Everything the room needs: is the cat out of its box, and which furniture does the student own?
public class SceneViewModel
{
    public bool CatOutOfBox { get; set; }
    public List<string> FurnitureSlugs { get; set; } = new();
}