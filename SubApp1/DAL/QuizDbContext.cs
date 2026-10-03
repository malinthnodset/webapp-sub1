using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizDbContext : DbContext
{
    // constructor - create empty schema based on model (?)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ensures course code if unique
    modelBuilder.Entity<Course>()
        .HasIndex(c => c.Code)
        .IsUnique();

    // Seed data (hard coded) for quiz generation (depends on exisiting courses and students/creators)
    modelBuilder.Entity<Student>().HasData(
        new Student { Id = 1, Name = "Test Student", Email = "test@example.com" });

    modelBuilder.Entity<Course>().HasData(
        new Course ("ITPE3200", "Web Applications") {Id = 1},
        new Course ("EX1000", "Example Course" ) {Id = 2});
}
}