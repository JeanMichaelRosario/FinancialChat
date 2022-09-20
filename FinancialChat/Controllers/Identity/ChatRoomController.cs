using FinancialChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Controllers.Identity
{
    [Authorize]
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatRoomController> _chatHub;
        private readonly ILogger<ChatRoomController> _logger;

        public ChatRoomController(ApplicationDbContext context, IHubContext<ChatRoomController> chatHub, ILogger<ChatRoomController> logger)
        {
            _context = context;
            _chatHub = chatHub;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Rooms = await _context.ChatRooms.ToListAsync();
            return PartialView();
        }

        public async Task<IActionResult> Chat(Guid chatId)
        {
            var chat = await _context.ChatRooms
                .Where(x => x.Id == chatId)
                .Include(x => x.Messages)
                .FirstAsync();

            chat.Messages = chat.Messages.OrderByDescending(x => x.CreatedAt).Take(50).Reverse().ToList();

            ViewBag.Chat = chat;

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatRoom(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var chat = new ChatRoom()
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                _context.ChatRooms.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest("The name of the chatroom cannot be empty");
        }

        [HttpGet("stock={stockCode}")]
        public async Task<IActionResult> PostMessage(string message)
        {
            try
            {
                var chatRoomMessage = new ChatRoomMessage()
                {
                    UserId = "0",
                    Message = message
                };

                await _chatHub.Clients.All.SendAsync("ReceiveMessage", chatRoomMessage);
                return RedirectToAction("Index", "ChatRoom");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error trying to get the stock value", ex);
            }
            return BadRequest("Stock not Found");
        }
    }
}
