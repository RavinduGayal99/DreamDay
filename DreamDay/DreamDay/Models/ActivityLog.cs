using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models
{
    public class ActivityLog
    {
        [Key] public int Id { get; set; }
        public string ActionDescription { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}