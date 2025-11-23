using DreamDay.Data;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        // --- USER MANAGEMENT ---
        public async Task<IActionResult> Users() => View(await _context.Users.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> CreateUser(string name, string email, string password, string role)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return RedirectToAction(nameof(Users));
            }
            var user = new User { Name = name, Email = email, UserName = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                if (role == "Couple") _context.Couples.Add(new Couple { UserId = user.Id });
                else if (role == "WeddingPlanner") _context.WeddingPlanners.Add(new WeddingPlanner { UserId = user.Id });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserDetailsViewModel
            {
                User = user,
                Roles = roles
            };
            return PartialView("_UserDetailsPartial", viewModel);
        }

        // --- VENDOR MANAGEMENT ---
        public async Task<IActionResult> Vendors() => View(await _context.Vendors.ToListAsync());
        public IActionResult CreateVendor() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVendor([Bind("Name,Category,ContactInfo,Description")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Vendors));
            }
            return View(vendor);
        }
        public async Task<IActionResult> EditVendor(int? id)
        {
            if (id == null) return NotFound();
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVendor(int id, [Bind("Id,Name,Category,ContactInfo,Description")] Vendor vendor)
        {
            if (id != vendor.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(vendor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Vendors));
            }
            return View(vendor);
        }
        public async Task<IActionResult> DeleteVendor(int? id)
        {
            if (id == null) return NotFound();
            var vendor = await _context.Vendors.FirstOrDefaultAsync(m => m.Id == id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }
        [HttpPost, ActionName("DeleteVendor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVendorConfirmed(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor != null)
            {
                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Vendors));
        }

        // --- SYSTEM DASHBOARD & REPORTS ---
        public async Task<IActionResult> SystemDashboard()
        {
            var usersByRole = await _context.UserRoles
                .GroupBy(ur => ur.RoleId)
                .Select(g => new { RoleId = g.Key, Count = g.Count() })
                .ToListAsync();

            var roleNames = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);

            var usersChart = new ChartData();
            foreach (var item in usersByRole)
            {
                if (roleNames.ContainsKey(item.RoleId))
                {
                    usersChart.Labels.Add(roleNames[item.RoleId]);
                    usersChart.Data.Add(item.Count);
                }
            }

            var weddingsByStatus = await _context.Weddings
                .GroupBy(w => w.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var weddingsChart = new ChartData();
            foreach (var item in weddingsByStatus)
            {
                weddingsChart.Labels.Add(item.Status);
                weddingsChart.Data.Add(item.Count);
            }

            var viewModel = new SystemDashboardViewModel
            {
                UsersByRoleChart = usersChart,
                WeddingsByStatusChart = weddingsChart
            };

            return View(viewModel);
        }

        public IActionResult Reports() => View();

        // FIX: This action now correctly creates and passes a list of ViewModels to the view.
        public async Task<IActionResult> GenerateUserReport()
        {
            var users = await _context.Users.ToListAsync();
            var userReportViewModels = new List<UserDetailsViewModel>();
            foreach (var user in users)
            {
                userReportViewModels.Add(new UserDetailsViewModel
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return View("UserReport", userReportViewModels);
        }

        public async Task<IActionResult> GenerateVendorReport()
        {
            var vendors = await _context.Vendors.ToListAsync();
            return View("VendorReport", vendors);
        }

        public async Task<IActionResult> GeneratePopularityReport()
        {
            var popularVenues = await _context.VendorBookings
                .Include(b => b.Vendor)
                .Where(b => b.Vendor.Category.ToLower() == "venue")
                .GroupBy(b => b.Vendor.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return View("PopularityReport", popularVenues);
        }

        public async Task<IActionResult> GenerateBudgetReport()
        {
            var budgets = await _context.Weddings
                .Select(w => new { w.Title, w.Budget })
                .ToListAsync();

            return View("AverageBudgetReport", budgets);
        }
    }
}