using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using Microsoft.EntityFrameworkCore.Design; // [LEARN] Design-time support

namespace SubApp1.DAL; // [LEARN] Data access namespace

// [LEARN] tells the EF core's command-line how to create a QuizDbContext
// [LEARN] when there is no running app to get one from
public class QuizDbContextFactory : IDesignTimeDbContextFactory<QuizDbContext> // [LEARN] Factory for CLI
{
    public QuizDbContext CreateDbContext(string[] args) // [LEARN] Create context
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); // [LEARN] Get env
        var configuration = new ConfigurationBuilder() // [LEARN] Build config
            .SetBasePath(Directory.GetCurrentDirectory()) // [LEARN] Base path
            .AddJsonFile("appsettings.json", optional: false) // [LEARN] Main settings
            .AddJsonFile($"appsettings.{environment}.json", optional: true) // [LEARN] Env settings
            .AddEnvironmentVariables() // [LEARN] Env vars
            .Build(); // [LEARN] Build

        var connectionString = configuration.GetConnectionString("DefaultConnection") // [LEARN] Get connection string
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found."); // [LEARN] Require it

        var options = new DbContextOptionsBuilder<QuizDbContext>() // [LEARN] Build options
            .UseSqlite(connectionString) // [LEARN] SQLite
            .Options; // [LEARN] Options

        return new QuizDbContext(options); // [LEARN] Return context
    }
}
