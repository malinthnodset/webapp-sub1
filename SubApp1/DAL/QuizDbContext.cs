using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizDbContext : DbContext
{
    // construvtor - create empty schema based on model (?)
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

    // Seed data (hard coded) for quiz generation (depends on exisiting courses and students/creators)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Student>().HasData(
        new Student { Id = 1, Name = "Test Student", Email = "test@example.com" });

    modelBuilder.Entity<Course>().HasData(
        new Course { Id = 1, Name = "Web Applications", Code = "ITPE3200" },
        new Course { Id = 2, Name = "Example Course", Code = "EX1000" });
}
}