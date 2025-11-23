using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "WeddingPlanner")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Shows all bookings for a specific wedding
        public async Task<IActionResult> Index(int weddingId)
        {
            var bookings = await _context.VendorBookings
                .Where(b => b.WeddingId == weddingId)
                .Include(b => b.Vendor)
                .ToListAsync();
            
            ViewBag.WeddingId = weddingId;
            ViewBag.WeddingTitle = (await _context.Weddings.FindAsync(weddingId))?.Title;
            return View(bookings);
        }

        // Page to create a new booking
        public async Task<IActionResult> Create(int weddingId)
        {
            ViewBag.WeddingId = weddingId;
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorBooking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { weddingId = booking.WeddingId });
            }
            ViewBag.WeddingId = booking.WeddingId;
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            return View(booking);
        }
    }
}