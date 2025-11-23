using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DreamDay.Controllers
{
    [Authorize(Roles = "Couple")]
    public class BudgetController : BaseController
    {
        public BudgetController(ApplicationDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return RedirectToAction("Create", "Wedding");

            var budget = await _context.Budgets
                .Include(b => b.BudgetCategories)
                    .ThenInclude(bc => bc.Expenses)
                .FirstOrDefaultAsync(b => b.WeddingId == wedding.Id);

            if (budget == null)
            {
                budget = new Budget { WeddingId = wedding.Id };
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
            }

            ViewBag.TotalBudget = wedding.Budget;
            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, decimal allocatedAmount)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.WeddingId == wedding.Id);

            var category = new BudgetCategory
            {
                BudgetId = budget.Id,
                Name = name,
                AllocatedAmount = allocatedAmount
            };
            _context.BudgetCategories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(int budgetCategoryId, string description, decimal amount)
        {
            var wedding = await GetCurrentUserWeddingAsync();
            if (wedding == null) return Forbid();

            var category = await _context.BudgetCategories
                .FirstOrDefaultAsync(c => c.Id == budgetCategoryId && c.Budget.WeddingId == wedding.Id);

            if (category == null) return Forbid();

            var expense = new Expense
            {
                BudgetCategoryId = budgetCategoryId,
                Description = description,
                Amount = amount,
                Date = DateTime.UtcNow
            };
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            await CheckForBudgetOverrunAndNotify(category, wedding.Couple.UserId);

            return RedirectToAction("Index");
        }

        private async Task CheckForBudgetOverrunAndNotify(BudgetCategory category, int coupleUserId)
        {
            var totalSpentInCategory = await _context.Expenses
                .Where(e => e.BudgetCategoryId == category.Id)
                .SumAsync(e => e.Amount);

            if (totalSpentInCategory > category.AllocatedAmount)
            {
                var existingNotification = await _context.Notifications
                    .AnyAsync(n => n.UserId == coupleUserId && n.Content.Contains($"'{category.Name}' category"));

                if (!existingNotification)
                {
                    var notification = new Notification
                    {
                        UserId = coupleUserId,
                        Title = "Budget Alert!",
                        Content = $"You have exceeded your budget for the '{category.Name}' category.",
                        Type = NotificationType.BUDGET_ALERT
                    };
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}