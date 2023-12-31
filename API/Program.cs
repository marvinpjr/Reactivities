using API.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);
string corsPolicyName = "CorsPolicy";

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration, corsPolicyName);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {}
app.UseCors(corsPolicyName);
app.UseAuthorization();
app.MapControllers();

// init database and seed data if it doesn't exist
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during database migration.");
}

app.Run();

