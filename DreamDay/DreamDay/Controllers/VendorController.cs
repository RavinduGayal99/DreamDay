using DreamDay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() => View(await _context.Vendors.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vendor = await _context.Vendors
                .Include(v => v.Services)
                .Include(v => v.Reviews)
                .ThenInclude(r => r.Couple.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (vendor == null) return NotFound();

            return View(vendor);
        }
    }
}