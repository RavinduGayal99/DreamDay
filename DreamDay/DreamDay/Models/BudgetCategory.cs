using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DreamDay.Models {
    public class BudgetCategory {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal AllocatedAmount { get; set; }
        public int BudgetId { get; set; }
        public virtual Budget Budget { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}