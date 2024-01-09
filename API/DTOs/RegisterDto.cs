using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // it is an object use to encapsulate data and send it from one subsystem of application to another
    // allow us to create different object and just send back properties that we're interested
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; } // optinal to make required work!
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 3)]
        public string Password { get; set; }
    }
}