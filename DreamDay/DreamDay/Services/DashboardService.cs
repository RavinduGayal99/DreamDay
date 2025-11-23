using DreamDay.Data;
using DreamDay.Models;
using DreamDay.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardViewModelAsync(int weddingId)
        {
            var wedding = await _context.Weddings
                .Include(w => w.ProjectTasks)
                .FirstOrDefaultAsync(w => w.Id == weddingId);

            if (wedding == null)
            {
                return null;
            }

            var totalExpenses = await _context.Expenses
                .Where(e => e.BudgetCategory.Budget.WeddingId == weddingId)
                .SumAsync(e => e.Amount);


            var viewModel = new DashboardViewModel
            {
                CurrentWedding = wedding,
                DaysRemaining = (wedding.WeddingDate - DateTime.Today).Days,
                UpcomingTasks = wedding.ProjectTasks.Where(t => !t.IsCompleted && t.DueDate >= DateTime.Today).OrderBy(t => t.DueDate).Take(5).ToList(),
                BudgetTotal = wedding.Budget,
                BudgetSpent = totalExpenses,
                TotalTasks = wedding.ProjectTasks.Count(),
                CompletedTasks = wedding.ProjectTasks.Count(t => t.IsCompleted)
            };

            if (viewModel.BudgetTotal > 0)
                viewModel.BudgetProgressPercentage = (viewModel.BudgetSpent / viewModel.BudgetTotal) * 100;

            if (viewModel.TotalTasks > 0)
                viewModel.TaskCompletionPercentage = ((double)viewModel.CompletedTasks / viewModel.TotalTasks) * 100;

            return viewModel;
        }
    }
}