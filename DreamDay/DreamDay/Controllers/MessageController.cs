using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var wedding = await _context.Weddings
                .FirstOrDefaultAsync(w => w.Couple.UserId == currentUser.Id || w.WeddingPlanner.UserId == currentUser.Id);

            if (wedding == null) return View(null);

            var couple = await _context.Couples.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == wedding.CoupleId);
            var planner = await _context.WeddingPlanners.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == wedding.WeddingPlannerId);

            User otherUser = null;
            if (User.IsInRole("Couple")) otherUser = planner?.User;
            else if (User.IsInRole("WeddingPlanner")) otherUser = couple?.User;

            if (otherUser == null) return View(null);

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == otherUser.Id) ||
                             (m.SenderId == otherUser.Id && m.ReceiverId == currentUser.Id))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.OtherUser = otherUser;
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send(string content, int receiverId)
        {
            var sender = await _userManager.GetUserAsync(User);
            var message = new Message { Content = content, SenderId = sender.Id, ReceiverId = receiverId };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}