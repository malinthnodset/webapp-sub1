using Microsoft.EntityFrameworkCore; // [LEARN] EF Core: object-relational mapper for .NET databases
using SubApp1.DAL; // [LEARN] Our data-access layer (DbContext, repositories)
using SubApp1.Services; // [LEARN] Business logic layer (services)
using Serilog; // [LEARN] Third-party logging library
using Serilog.Events; // [LEARN] Log level enum (Information, Warning, etc.)

var builder = WebApplication.CreateBuilder(args); // [LEARN] Creates the web app builder and reads appsettings.json

// [LEARN] services (ASP.NET components) for handling controllers and views to dependency injection container 
// [LEARN] sets up the MVC pattern for handling HTTP requests
builder.Services.AddControllersWithViews(); // [LEARN] Registers MVC controllers + Razor views with DI
builder.Services.AddScoped<DashboardService>(); // [LEARN] DI: one DashboardService per HTTP request
builder.Services.AddScoped<IQuizService, QuizService>(); // [LEARN] DI: inject QuizService when IQuizService is requested

// [LEARN] DATABASE

// [LEARN] dependency injection  for DB
builder.Services.AddDbContext<QuizDbContext>(options => // [LEARN] Registers EF DbContext with DI
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // [LEARN] Use SQLite with connection string from appsettings.json

// [LEARN] registering the repositories (in DAL)
builder.Services.AddScoped<IQuizRepository, QuizRepository>(); // [LEARN] DI: QuizRepository implements IQuizRepository
builder.Services.AddScoped<ICourseRepository, CourseRepository>(); // [LEARN] DI: CourseRepository implements ICourseRepository

// [LEARN] LOGGER
builder.Services.AddSerilog((services, loggerConfiguration) => // [LEARN] Configure Serilog as the logging provider
{
    loggerConfiguration // [LEARN] Logger settings
        .MinimumLevel.Information() // [LEARN] Log at Information level and above
        .WriteTo.Console() // can be commented out for less noise in console
        .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log") // [LEARN] Write logs to a timestamped file
        // [LEARN] filter out info-level EF db execution logs
        .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContexT", out var value) && // [LEARN] Exclude EF "Executed DbCommand" info logs
            e.Level == LogEventLevel.Information && // [LEARN] Only filter Information-level events
            e.MessageTemplate.Text.Contains("Executed DbCommand")); // [LEARN] Match EF's command-executed message
});

var app = builder.Build(); // [LEARN] Build the configured web application

if (app.Environment.IsDevelopment()) // [LEARN] Only seed DB in Development environment
{
    DBInit.Seed(app); // [LEARN] Apply migrations + seed sample data
    app.UseDeveloperExceptionPage(); // [LEARN] Show detailed error page in dev
}

// [LEARN] Enable static assets from wwwroot (images, JS, CSS)
app.MapStaticAssets(); // [LEARN] Serve files from wwwroot

app.MapControllerRoute( // [LEARN] Define default MVC route
    name: "default", // [LEARN] Route name
    pattern: "{controller=Dashboard}/{action=Index}/{id?}"); // [LEARN] Default: DashboardController.Index, optional id
app.Run(); // [LEARN] Start the web server and block until shutdown
