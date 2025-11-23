using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple")]
    public class WeddingController : BaseController
    {
        public WeddingController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Wedding wedding)
        {
            if (ModelState.IsValid)
            {
                var couple = await _context.Couples.FirstOrDefaultAsync(c => c.UserId == int.Parse(_userManager.GetUserId(User)));
                if (couple == null) return Unauthorized();

                wedding.CoupleId = couple.Id;
                _context.Add(wedding);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }
            return View(wedding);
        }
    }
}