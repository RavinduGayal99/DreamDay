using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public enum GuestStatus { INVITED, ATTENDING, DECLINED, PENDING }
    public class Guest {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string? Email { get; set; }
        public GuestStatus RsvpStatus { get; set; } = GuestStatus.PENDING;
        public string? MealPreference { get; set; }
        public int WeddingId { get; set; }
        public virtual Wedding Wedding { get; set; }
        public int? TableId { get; set; }
        public virtual Table? Table { get; set; }
    }
}