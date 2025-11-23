using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple")]
    public class TimelineController : BaseController
    {
        public TimelineController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return RedirectToAction("Create", "Wedding");

            var timeline = await _context.Timelines
                .Include(t => t.TimelineEvents.OrderBy(te => te.StartTime))
                .FirstOrDefaultAsync(t => t.WeddingId == wedding.Id);

            if (timeline == null)
            {
                timeline = new Timeline { WeddingId = wedding.Id };
                _context.Timelines.Add(timeline);
                await _context.SaveChangesAsync();
            }

            return View(timeline);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent(string title, DateTime startTime)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            var timeline = await _context.Timelines.FirstOrDefaultAsync(t => t.WeddingId == wedding.Id);

            var newEvent = new TimelineEvent
            {
                TimelineId = timeline.Id,
                Title = title,
                StartTime = startTime,
                EndTime = startTime.AddHours(1)
            };
            _context.TimelineEvents.Add(newEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}