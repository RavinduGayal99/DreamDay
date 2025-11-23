using DreamDay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "WeddingPlanner")]
    public class PlannerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.User> _userManager;

        public PlannerController(ApplicationDbContext context, UserManager<Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var planner = await _context.WeddingPlanners.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (planner == null) return Unauthorized();

            var weddings = await _context.Weddings
                .Where(w => w.WeddingPlannerId == planner.Id)
                .Include(w => w.Couple)
                    .ThenInclude(c => c.User)
                .ToListAsync();

            return View(weddings);
        }
    }
}