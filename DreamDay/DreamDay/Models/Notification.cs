using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public enum NotificationType { TASK_DUE, BUDGET_ALERT, RSVP_UPDATE, VENDOR_MESSAGE, SYSTEM_ALERT }
    public class Notification {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}