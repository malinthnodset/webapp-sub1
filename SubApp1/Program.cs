using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;

var builder = WebApplication.CreateBuilder(args);

// services (ASP.NET components) for handling controllers and views to dependency injection container 
// sets up the MVC pattern for handling HTTP requests
builder.Services.AddControllersWithViews();

// dependency injection 
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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