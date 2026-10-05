using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizDbContext : DbContext
{
    // creates constructor that takes in options, and sends them to DbContext with :base(options)
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

// configures the model, runs once when EF buils the model of the db
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);

    // ensures course code is unique
    modelBuilder.Entity<Course>()
        .HasIndex(c => c.Code)
        .IsUnique();
    }
}