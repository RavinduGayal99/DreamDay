using DreamDay.Models;
using ProjectTask = DreamDay.Models.ProjectTask; // <-- ADD THIS ALIAS

namespace DreamDay.ViewModels
{
    public class DashboardViewModel
    {
        public Wedding CurrentWedding { get; set; }
        public int DaysRemaining { get; set; }

        // --- THIS LINE IS FIXED ---
        public List<ProjectTask> UpcomingTasks { get; set; }
        // --------------------------

        public decimal BudgetTotal { get; set; }
        public decimal BudgetSpent { get; set; }
        public decimal BudgetProgressPercentage { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double TaskCompletionPercentage { get; set; }
    }
}