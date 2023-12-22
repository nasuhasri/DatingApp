using System.Text;
using API.Data;
using API.interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // specify all the rules on how the server should validates the token is a good token
        options.TokenValidationParameters = new TokenValidationParameters {
            // check token signing key based on the issuer signing key
            // if not, anybody can create any random token as long as its JWT
            ValidateIssuerSigningKey = true,
            // specify what is our issuer signing key is
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            // both false bcs we do not have the information in our token
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication(); // do you have valid token?
app.UseAuthorization(); // user have valid token, what user is allowed to do?

app.MapControllers();

app.Run();
