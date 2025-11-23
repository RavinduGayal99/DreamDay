using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Message {
        [Key] public int Id { get; set; }
        [Required] public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}