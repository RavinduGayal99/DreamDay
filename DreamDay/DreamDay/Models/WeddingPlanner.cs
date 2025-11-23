using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DreamDay.Models {
    public class WeddingPlanner {
        [Key] public int Id { get; set; }
        public string? Experience { get; set; }
        public string? Specialization { get; set; }
        public string? BusinessName { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")] public virtual User User { get; set; }
        public virtual ICollection<Wedding> ManagedWeddings { get; set; } = new List<Wedding>();
    }
}