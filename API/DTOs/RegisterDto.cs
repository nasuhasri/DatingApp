namespace API.DTOs
{
    // it is an object use to encapsulate data and send it from one subsystem of application to another
    // allow us to create different object and just send back properties that we're interested
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}