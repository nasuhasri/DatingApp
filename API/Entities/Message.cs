namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int ReceipentId { get; set; }
        public string ReceipentUsername { get; set; }
        public AppUser Receipent { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        /*
            - If a sender of this message decides to delete message from their side, they dont have control over the inbox of the user they send their message to
            - So if both of these properties set to True, only then we will delete the message from db
        */
        public bool SenderDeleted { get; set; }
        public bool ReceipentDeleted { get; set; }
    }
}