using DreamDay.Data;
using DreamDay.Models;
using DreamDay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple, WeddingPlanner")]
    public class DashboardController : BaseController
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager, DashboardService dashboardService)
            : base(context, userManager)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(int? weddingId)
        {
            Wedding wedding;
            var user = await _userManager.GetUserAsync(User);
            var plannerProfile = User.IsInRole("WeddingPlanner")
                ? await _context.WeddingPlanners.FirstOrDefaultAsync(p => p.UserId == user.Id)
                : null;

            if (weddingId.HasValue)
            {
                wedding = await _context.Weddings.FindAsync(weddingId.Value);
                if (wedding == null) return NotFound();

                bool isAuthorized = (User.IsInRole("Couple") && (await GetCurrentUserWeddingAsync())?.Id == weddingId) ||
                                    (User.IsInRole("WeddingPlanner") && wedding.WeddingPlannerId == plannerProfile?.Id);

                if (!isAuthorized) return Forbid();
            }
            else
            {
                if (!User.IsInRole("Couple")) return Forbid();
                wedding = await GetCurrentUserWeddingAsync();
            }

            if (wedding == null)
            {
                return RedirectToAction("Create", "Wedding");
            }

            var viewModel = await _dashboardService.GetDashboardViewModelAsync(wedding.Id);
            return View(viewModel);
        }
    }
}