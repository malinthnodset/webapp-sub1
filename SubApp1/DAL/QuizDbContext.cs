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
}