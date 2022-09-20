using Microsoft.AspNetCore.Identity;

namespace FinancialChat.Data
{
    public class User : IdentityUser
    {
        public ICollection<ChatRoom> ChatRooms { get; set; }
        public ICollection<ChatRoomMessage> Messages { get; set; }
    }
}
