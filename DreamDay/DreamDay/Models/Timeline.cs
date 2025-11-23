using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Timeline {
        [Key] public int Id { get; set; }
        public int WeddingId { get; set; }
        public virtual Wedding Wedding { get; set; }
        public virtual ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
    }
}