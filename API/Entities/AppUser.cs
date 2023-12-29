using System.Text.Json.Serialization;
using API.Extensions;
using API.Helpers;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    // DateOnly only display date not included time. DateTime will include time.
    public DateOnly DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    // always use utc which equivalent to gmt time especially when handling user from different time zones
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public List<Photo> Photos { get; set; } = new List<Photo>();

    // public int GetAge() {
    //     return DateOfBirth.CalculateAge();
    // }
}