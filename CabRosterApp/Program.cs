using CabRosterApp.Data;
using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure DbContext to use SQL Server connection string from configuration
builder.Services.AddDbContext<CabRosterAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with custom options for unique email
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CabRosterAppDbContext>(); // Use EF to store user data

// Configure CORS to allow any origin (adjust for production environment)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()  // Allow any origin
              .AllowAnyMethod()  // Allow any HTTP method
              .AllowAnyHeader()); // Allow any HTTP header
});

// Add controllers (API endpoints)
builder.Services.AddControllers();

// Set up Swagger for API documentation (optional but recommended in development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database and initialize roles (run only once)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

    // Seed data to create the initial roles and admin user (if necessary)
    await SeedData.Initialize(services, userManager, roleManager, logger);
}

app.UseCors("AllowAll"); // Apply the CORS policy

// Enable middleware for HTTP requests

// Use HTTPS redirection for production and development environments
app.UseHttpsRedirection();

// Set up routing
app.UseRouting();

// Apply the CORS policy
app.UseCors("AllowAllOrigins");

// Map API controllers
app.MapControllers();

// Enable Swagger UI (only in development environment)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Serve the Swagger JSON for API docs
    app.UseSwaggerUI(); // Serve the Swagger UI for testing APIs interactively
}

// Run the application
app.Run();
