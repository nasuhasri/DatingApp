using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        // In an extension method 'this' refers to the thing that we are extending.
        // In this case we are extending the IServiceCollection by adding another method onto it so 'this' refers to the IServiceCollection
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt => {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>(); // create all tables related to identity in db

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    // specify all the rules on how the server should validates the token is a good token
                    options.TokenValidationParameters = new TokenValidationParameters {
                        // check token signing key based on the issuer signing key
                        // if not, anybody can create any random token as long as its JWT
                        ValidateIssuerSigningKey = true,
                        // specify what is our issuer signing key is
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        // both false bcs we do not have the information in our token
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            // access_token: var that signalR will use from the client side when it sends up token
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            // if we are on that path and have access token
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
                                // gives signalR or hub to the bearer token bcs we adding it to context
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
    }
}