namespace FinancialChat.Data
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        public ICollection<User> Users { get; set; }
        public ICollection<ChatRoomMessage> Messages { get; set; }
    }
}
