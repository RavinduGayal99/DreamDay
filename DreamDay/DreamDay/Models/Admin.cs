using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DreamDay.Models {
    public class Admin {
        [Key] public int Id { get; set; }
        public string Role { get; set; } = "SystemAdmin";
        public int UserId { get; set; }
        [ForeignKey("UserId")] public virtual User User { get; set; }
    }
}