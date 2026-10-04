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

    // ensures course code is unique
    modelBuilder.Entity<Course>()
        .HasIndex(c => c.Code)
        .IsUnique();

    // Seed data (hard coded) for quiz generation (depends on exisiting courses and students/creators)
    modelBuilder.Entity<Student>().HasData(
        new Student { Id = 1, Name = "Test Student", Email = "test@example.com" });

    modelBuilder.Entity<Course>().HasData(
        new Course ("ITPE3200", "Web Applications") {Id = 1},
        new Course ("EX1000", "Example Course" ) {Id = 2});

    // Seed data - hardcoded quiz for test
    modelBuilder.Entity<Quiz>().HasData(
        new Quiz
        {
            Id = 1000,
            Title = "Web basics",
            Description = "A ten-question introduction to web development.",
            CourseId = 1,
            CreatedByStudentId = 1
        });

    // hardcoded questions for test quiz
    modelBuilder.Entity<QuizQuestion>().HasData(
        new QuizQuestion { Id = 10001, QuizId = 1000, Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 },
        new QuizQuestion { Id = 10002, QuizId = 1000, Prompt = "What is 2 + 2?", CorrectAnswer = "4", Points = 1, Order = 2 },
        new QuizQuestion { Id = 10003, QuizId = 1000, Prompt = "What does HTML stand for?", CorrectAnswer = "HyperText Markup Language", Points = 1, Order = 3 },
        new QuizQuestion { Id = 10004, QuizId = 1000, Prompt = "What does CSS stand for?", CorrectAnswer = "Cascading Style Sheets", Points = 1, Order = 4 },
        new QuizQuestion { Id = 10005, QuizId = 1000, Prompt = "Which language is commonly used to add interactivity to webpages?", CorrectAnswer = "JavaScript", Points = 1, Order = 5 },
        new QuizQuestion { Id = 10006, QuizId = 1000, Prompt = "Which HTTP method is commonly used to retrieve data?", CorrectAnswer = "GET", Points = 1, Order = 6 },
        new QuizQuestion { Id = 10007, QuizId = 1000, Prompt = "Which HTTP method is commonly used to submit data?", CorrectAnswer = "POST", Points = 1, Order = 7 },
        new QuizQuestion { Id = 10008, QuizId = 1000, Prompt = "What does URL stand for?", CorrectAnswer = "Uniform Resource Locator", Points = 1, Order = 8 },
        new QuizQuestion { Id = 10009, QuizId = 1000, Prompt = "Which HTTP status code indicates a successful request?", CorrectAnswer = "200", Points = 1, Order = 9 },
        new QuizQuestion { Id = 10010, QuizId = 1000, Prompt = "Which HTTP status code means a requested resource was not found?", CorrectAnswer = "404", Points = 1, Order = 10 }
    );

}
}