using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple")]
    public class GuestController : BaseController
    {
        public GuestController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return RedirectToAction("Create", "Wedding");

            var guests = await _context.Guests
                .Where(g => g.WeddingId == wedding.Id)
                .ToListAsync();

            return View(guests);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,MealPreference")] Guest guest)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return NotFound();

            if (ModelState.IsValid)
            {
                guest.WeddingId = wedding.Id;
                guest.RsvpStatus = GuestStatus.INVITED;
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            
            var wedding = await GetCurrentUserWeddingAsync();
            if (guest.WeddingId != wedding.Id) return Forbid();

            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,RsvpStatus,MealPreference,WeddingId")] Guest guest)
        {
            if (id != guest.Id) return NotFound();
            
            var wedding = await GetCurrentUserWeddingAsync();
            if (guest.WeddingId != wedding.Id) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Update(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var guest = await _context.Guests.FirstOrDefaultAsync(m => m.Id == id);
            if (guest == null) return NotFound();

            var wedding = await GetCurrentUserWeddingAsync();
            if (guest.WeddingId != wedding.Id) return Forbid();

            return View(guest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            var wedding = await GetCurrentUserWeddingAsync();
            if (guest.WeddingId != wedding.Id) return Forbid();

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}