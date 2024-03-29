using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication(); // do you have valid token?
app.UseAuthorization(); // user have valid token, what user is allowed to do?

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

// give access to all the services we have inside the program class
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    
    // MigrateAsync(): Asynchronously applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    await context.Database.MigrateAsync();

    // options 1 to clear the Connections table: but will cause error if we have thousands row of data
    // context.Connections.RemoveRange(context.Connections);

    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");

    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration!");
}

app.Run();
