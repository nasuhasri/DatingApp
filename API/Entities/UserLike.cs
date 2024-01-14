namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        // one who's liked by source user
        public AppUser TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}