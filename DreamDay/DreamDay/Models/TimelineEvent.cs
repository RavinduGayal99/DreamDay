using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class TimelineEvent {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TimelineId { get; set; }
        public virtual Timeline Timeline { get; set; }
    }
}