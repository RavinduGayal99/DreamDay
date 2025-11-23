using DreamDay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "WeddingPlanner, Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> BudgetReport(int weddingId)
        {
            var wedding = await _context.Weddings
                .Include(w => w.Couple.User)
                .FirstOrDefaultAsync(w => w.Id == weddingId);

            var budget = await _context.Budgets
                .Include(b => b.BudgetCategories)
                .ThenInclude(bc => bc.Expenses)
                .FirstOrDefaultAsync(b => b.WeddingId == weddingId);

            ViewBag.Wedding = wedding;
            return View(budget);
        }
    }
}