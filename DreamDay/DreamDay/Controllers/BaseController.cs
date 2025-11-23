using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<User> _userManager;

        protected BaseController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected async Task<Wedding> GetCurrentUserWeddingAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var couple = await _context.Couples.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (couple == null) return null;

            return await _context.Weddings
                .Include(w => w.Couple)
                .FirstOrDefaultAsync(w => w.CoupleId == couple.Id);
        }
    }
}