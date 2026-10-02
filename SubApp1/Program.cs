using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;

var builder = WebApplication.CreateBuilder(args);

// services (ASP.NET components) for handling controllers and views to dependency injection container 
// sets up the MVC pattern for handling HTTP requests
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SubApp1.Services.IQuizService, SubApp1.Services.InMemoryQuizService>(); // Replace with persistent storage later.

// dependency injection 
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// registering the QuizRepository-db (in DAL)
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapStaticAssets(); // Enable static assets from wwwroot (images, JS, CSS)

app.MapDefaultControllerRoute();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();