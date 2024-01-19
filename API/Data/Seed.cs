using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager) 
        {
            // if there's user, we will not seed data
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };

            // Deserialize bcs we want convert json object into C# object
            // and speciy it as a list of user
            // options var can be in the second parameter
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users) {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                // create password for each user
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                // user.PasswordSalt = hmac.Key;

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }   
    }
}