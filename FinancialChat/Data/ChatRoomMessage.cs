using System.ComponentModel.DataAnnotations;

namespace FinancialChat.Data
{
    public class ChatRoomMessage
    {
        [Key]
        public int Id { get; set; }
        public Guid ChatRoomId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
