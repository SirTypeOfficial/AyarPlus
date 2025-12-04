using AyarPlus.API.Data;
using AyarPlus.API.Middleware;
using AyarPlus.API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SQLite";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (databaseProvider.ToLower())
    {
        case "sqlserver":
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            break;
        case "postgresql":
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
            break;
        case "sqlite":
        default:
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
            break;
    }
});

// Services
builder.Services.AddScoped<IFileService, FileService>();

// Controllers
builder.Services.AddControllers();

// OpenAPI (.NET 10 native)
builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure wwwroot/uploads directory exists
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "contacts");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseHttpsRedirection();
app.UseStaticFiles();

// Map documentation endpoints before API Key middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("AyarPlus Contact API")
               .WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseApiKeyAuthentication();

app.UseAuthorization();

app.MapControllers();

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
