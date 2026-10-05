using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;
using SubApp1.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// services (ASP.NET components) for handling controllers and views to dependency injection container 
// sets up the MVC pattern for handling HTTP requests
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IQuizService, QuizService>();

// DATABASE

// dependency injection  for DB
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// registering the repositories (in DAL)
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// LOGGER
builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console() // can be commented out for less noise in console
        .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log")
        // filter out info-level EF db execution logs
        .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContexT", out var value) &&
            e.Level == LogEventLevel.Information &&
            e.MessageTemplate.Text.Contains("Executed DbCommand"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    DBInit.Seed(app);
    app.UseDeveloperExceptionPage();
}

// Enable static assets from wwwroot (images, JS, CSS)
app.MapStaticAssets(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();