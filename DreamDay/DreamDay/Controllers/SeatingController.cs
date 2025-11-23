using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple")]
    public class SeatingController : BaseController
    {
        public SeatingController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return RedirectToAction("Create", "Wedding");

            ViewBag.Tables = await _context.Tables
                .Where(t => t.WeddingId == wedding.Id)
                .Include(t => t.SeatedGuests)
                .ToListAsync();

            ViewBag.UnseatedGuests = new SelectList(
                await _context.Guests.Where(g => g.WeddingId == wedding.Id && g.RsvpStatus == GuestStatus.ATTENDING && g.TableId == null).ToListAsync(),
                "Id", "Name");

            ViewBag.TableList = new SelectList(
                await _context.Tables.Where(t => t.WeddingId == wedding.Id).ToListAsync(),
                "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTable(string name, int capacity)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            var table = new Table { Name = name, Capacity = capacity, WeddingId = wedding.Id };
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignGuest(int guestId, int tableId)
        {
            var guest = await _context.Guests.FindAsync(guestId);
            var table = await _context.Tables.Include(t => t.SeatedGuests).FirstOrDefaultAsync(t => t.Id == tableId);

            if (guest != null && table != null && table.SeatedGuests.Count < table.Capacity)
            {
                guest.TableId = tableId;
                _context.Update(guest);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}