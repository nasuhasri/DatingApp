using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")] // what name we want to put in database
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }

    // relationship property
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}