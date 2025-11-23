using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DreamDay.Models {
    public class Couple {
        [Key] public int Id { get; set; }
        public bool IsPremium { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")] public virtual User User { get; set; }
        public virtual ICollection<Wedding> Weddings { get; set; } = new List<Wedding>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}