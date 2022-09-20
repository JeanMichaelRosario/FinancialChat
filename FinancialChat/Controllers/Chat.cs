using FinancialChat.Data;
using Microsoft.AspNetCore.SignalR;

namespace FinancialChat.Controllers
{
    public class Chat : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Chat> _logger;

        public Chat(ApplicationDbContext context, ILogger<Chat> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendMessage(ChatMessage message)
        {
            var saved = await AddNewMessageToDb(message);

            if (saved)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
        }

        private async Task<bool> AddNewMessageToDb(ChatMessage message)
        {
            try
            {
                ChatRoomMessage newMessage = new ChatRoomMessage()
                {
                    UserId = message.UserId,
                    ChatRoomId = message.ChatId,
                    Message = message.Message,
                    CreatedAt = DateTime.Now
                };

                message.CreatedAt = newMessage.CreatedAt;
                message.DayOfWeek = message.CreatedAt.DayOfWeek.ToString();

                _context.ChatRoomMessages.Add(newMessage);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, ex.InnerException);
                throw;
            }
        }
    }

    public class ChatMessage
    {
        public string UserId { get; set; }
        public Guid ChatId { get; set; }
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
        public string DayOfWeek { get; set; }
    }
}
