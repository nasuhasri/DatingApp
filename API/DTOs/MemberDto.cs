using System.Text.Json.Serialization;
using API.Helpers;

namespace API.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        // DateOnly only display date not included time. DateTime will include time.
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public int Age { get; set; }
        public string KnownAs { get; set; }
        // always use utc which equivalent to gmt time especially when handling user from different time zones
        public DateTime DateCreated { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<PhotoDto> Photos { get; set; }
    }
}