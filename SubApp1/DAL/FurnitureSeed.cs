using SubApp1.Models;

namespace SubApp1.DAL;

public static class FurnitureSeed
{
    // Adds the furniture list if the table is empty. Safe to run on every start. 
    public static void Seed(QuizDbContext db)
    {
        if (db.FurnitureItems.Any()) return;

        db.FurnitureItems.AddRange(
            new Furniture { Name = "Cat tree", Slug = "cat-tree", SortOrder = 1 },
            new Furniture { Name = "Cat bed", Slug = "bed", SortOrder = 2 },
            new Furniture { Name = "Plant", Slug = "plant", SortOrder = 3 },
            new Furniture { Name = "Ball of yarn", Slug = "yarn", SortOrder = 4 },
            new Furniture { Name = "Mona Lisa", Slug = "painting-mona-lisa", SortOrder = 5 },
            new Furniture { Name = "The Great Wave", Slug = "painting-wave", SortOrder = 6 }
        
        );
        db.SaveChanges();
    }
}