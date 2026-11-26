using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizDbContext : IdentityDbContext
{
    // Creates constructor that takes in options, and sends them to DbContext with :base(options)
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
    }

    // db tables
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizResult> QuizResults => Set<QuizResult>();

    public DbSet<Furniture> FurnitureItems => Set<Furniture>();

    public DbSet<StudentFurniture> StudentFurnitures => Set<StudentFurniture>();

    // Configures the model, runs once when EF buils the model of the db
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ensures course codes are unique
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // a slug is unique, and a student can own each furniture only once
        modelBuilder.Entity<Furniture>()
            .HasIndex(f => f.Slug)
            .IsUnique();

        modelBuilder.Entity<StudentFurniture>()
            .HasIndex(sf => new { sf.StudentId, sf.FurnitureId })
            .IsUnique();
    }
}