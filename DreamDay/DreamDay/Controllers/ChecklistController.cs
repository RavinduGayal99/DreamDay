using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ProjectTask = DreamDay.Models.ProjectTask; // <-- ADD THIS ALIAS

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple, WeddingPlanner")]
    public class ChecklistController : BaseController
    {
        public ChecklistController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index(int? weddingId)
        {
            var wedding = await GetWeddingForAuthorizedUser(weddingId);
            if (wedding == null) return Forbid();

            var tasks = await _context.Tasks
                .Where(t => t.WeddingId == wedding.Id)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            ViewBag.WeddingId = wedding.Id;
            ViewBag.WeddingTitle = wedding.Title;
            return View(tasks);
        }

        public async Task<IActionResult> Create()
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return NotFound();
            // --- THIS LINE IS FIXED ---
            return View(new ProjectTask { WeddingId = wedding.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // --- THIS LINE IS FIXED ---
        public async Task<IActionResult> Create([Bind("Description,DueDate,Category,WeddingId")] ProjectTask task)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null || wedding.Id != task.WeddingId) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null || task.WeddingId != wedding.Id) return Forbid();

            task.IsCompleted = !task.IsCompleted;
            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Wedding> GetWeddingForAuthorizedUser(int? weddingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Couple"))
            {
                return await GetCurrentUserWeddingAsync();
            }
            if (User.IsInRole("WeddingPlanner") && weddingId.HasValue)
            {
                var planner = await _context.WeddingPlanners.FirstOrDefaultAsync(p => p.UserId == user.Id);
                var wedding = await _context.Weddings.FirstOrDefaultAsync(w => w.Id == weddingId.Value && w.WeddingPlannerId == planner.Id);
                return wedding;
            }
            return null;
        }
    }
}