using API.Data;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();
// lifetime on how long do we want the services to be available
/*
* 
1. AddTransient() 
    - short live service
    - the token service will be created and disposed within the request as soon as it's used and finished
    - when controller disposed at the end of the HTTP request, any dependent services also disposed
2. AddSingleton()
    - create service that's instantiated when application first starts
    - never dispose until application close down
3. AddScoped()
*/
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
