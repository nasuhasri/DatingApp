using System.Text.Json.Serialization;
using API.Extensions;
using API.Helpers;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
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
    public List<UserLike> LikedByUsers { get; set; }
    public List<UserLike> LikedUsers { get; set; }
    public List<Message> MessagesSent { get; set; }
    public List<Message> MessagesReceived { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }

    // public int GetAge() {
    //     return DateOfBirth.CalculateAge();
    // }
}