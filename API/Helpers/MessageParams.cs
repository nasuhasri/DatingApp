namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        // current logged in user
        public string Username { get; set; }
        // return unread message by default
        public string Container { get; set; } = "Unread";
    }
}