using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DreamDay.Models {
    public class Expense {
        [Key] public int Id { get; set; }
        [Required] public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int BudgetCategoryId { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}