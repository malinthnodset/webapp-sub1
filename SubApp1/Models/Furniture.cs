using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// A piece of furtniture that can be unlocked. Slug = file/CSS name, e.g. "cat-tree"
public class Furniture
{
    public int Id { get; set;}

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    // Decides the order in which furniture is unlocked
    public int SortOrder { get; set; }

    public ICollection<StudentFurniture> Owners { get; set; } = new List<StudentFurniture>();
}