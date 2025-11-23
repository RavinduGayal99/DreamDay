using DreamDay.Models;
using System.Collections.Generic;

namespace DreamDay.ViewModels
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class SystemDashboardViewModel
    {
        public ChartData UsersByRoleChart { get; set; }
        public ChartData WeddingsByStatusChart { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>();
    }
}