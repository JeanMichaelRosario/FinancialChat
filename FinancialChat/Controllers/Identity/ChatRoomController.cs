﻿using FinancialChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Controllers.Identity
{
    [Authorize]
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatRoomController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
