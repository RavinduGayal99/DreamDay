using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public enum ReportType { BUDGET, GUEST, VENDOR, PROGRESS, SYSTEM }
    public class Report {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public ReportType Type { get; set; }
        public string? ContentUrl { get; set; }
        public int GeneratedById { get; set; }
        public virtual User GeneratedBy { get; set; }
    }
}