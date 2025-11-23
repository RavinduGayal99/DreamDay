using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Budget {
        [Key] public int Id { get; set; }
        public int WeddingId { get; set; }
        public virtual Wedding Wedding { get; set; }
        public virtual ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();
    }
}