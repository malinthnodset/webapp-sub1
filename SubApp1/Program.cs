using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;
using SubApp1.Services;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;
using SubApp1.Models;

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<QuizDbContext>()
.AddDefaultTokenProviders();

// registering the repositories (in DAL)
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// registering the services (in Services)
builder.Services.AddScoped<IFurnitureService, FurnitureService>();

// LOGGER
builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console() // can be commented out for less noise in console
        .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log")
        // filter out info-level EF db execution logs
        .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
            e.Level == LogEventLevel.Information &&
            e.MessageTemplate.Text.Contains("Executed DbCommand"));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await DBInit.SeedAsync(app);
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    foreach (var role in Enum.GetNames<Role>())
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
}

// Enable static assets from wwwroot (images, JS, CSS)
//app.MapStaticAssets(); 
app.UseStaticFiles();

app.UseSession(); // 4. Add Session Middleware (Must be placed before UseAuthentication)

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Index}/{id?}");  //Makes the first page the user gets directed to "Welcome"
app.Run();