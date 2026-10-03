using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// services (ASP.NET components) for handling controllers and views to dependency injection container 
// sets up the MVC pattern for handling HTTP requests
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SubApp1.Services.IQuizService, SubApp1.Services.DatabaseQuizService>();
builder.Services.AddScoped<SubApp1.Services.DashboardService>();

// dependency injection 
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// registering the QuizRepository-db (in DAL)
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapStaticAssets(); // Enable static assets from wwwroot (images, JS, CSS)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();