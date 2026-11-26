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
builder.Services.AddScoped<IFurnitureService, FurnitureService>();

// DATABASE
// dependency injection  for DB
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// AUTH
// Legger inn identitetsrammeverk for innlogging 
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // disable email confirmation for easy local testing
    options.User.RequireUniqueEmail = true; // Sofie - flyttet opp fra kommentert ut kode nedenfor

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<QuizDbContext>();

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

// Sofie - kommentert ut midlertidig for å få programmet til å kjøre. La epost-sjekken opp i AddIdentity over
// Sørger for at e-postadressen til brukeren er unik i databasen
// builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
// {
//     options.User.RequireUniqueEmail = true;
// })
// .AddEntityFrameworkStores<QuizDbContext>()
// .AddDefaultTokenProviders();

// DAL - registering repositories
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
// TODO: add user repo

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

// Seed roles into the database if they don't exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in Enum.GetNames<Role>())
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

if (app.Environment.IsDevelopment())
{
    DBInit.Seed(app);
    app.UseDeveloperExceptionPage();
}

// Furniture list is seeded in all environments, without touching existing rows
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    FurnitureSeed.Seed(db);
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