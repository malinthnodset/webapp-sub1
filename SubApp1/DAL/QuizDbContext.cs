using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

public class QuizDbContext : DbContext // [LEARN] EF Core database context
{
    // [LEARN] Creates constructor that takes in options, and sends them to DbContext with :base(options)
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) // [LEARN] Constructor
    {
    }

    // [LEARN] db tables
    public DbSet<Course> Courses => Set<Course>(); // [LEARN] Courses table
    public DbSet<Student> Students => Set<Student>(); // [LEARN] Students table
    public DbSet<Quiz> Quizzes => Set<Quiz>(); // [LEARN] Quizzes table
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>(); // [LEARN] Questions table
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>(); // [LEARN] Answers table
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>(); // [LEARN] Attempts table
    public DbSet<QuizResult> QuizResults => Set<QuizResult>(); // [LEARN] Results table

    // [LEARN] Configures the model, runs once when EF buils the model of the db
    protected override void OnModelCreating(ModelBuilder modelBuilder) // [LEARN] Model configuration
    {
    base.OnModelCreating(modelBuilder); // [LEARN] Base config

    // [LEARN] ensures course code is unique
    modelBuilder.Entity<Course>() // [LEARN] Course entity
        .HasIndex(c => c.Code) // [LEARN] Index on Code
        .IsUnique(); // [LEARN] Unique constraint
    }
}
