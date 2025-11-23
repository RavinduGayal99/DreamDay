using System.ComponentModel.DataAnnotations;
using ProjectTask = DreamDay.Models.ProjectTask; // <-- ADD THIS ALIAS

namespace DreamDay.Models
{
    public enum WeddingStatus { PLANNING, UPCOMING, COMPLETED, CANCELLED }
    public class Wedding
    {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; }
        public DateTime WeddingDate { get; set; }
        public string? Theme { get; set; }
        public string? Location { get; set; }
        public decimal Budget { get; set; }
        public WeddingStatus Status { get; set; }
        public int CoupleId { get; set; }
        public int? WeddingPlannerId { get; set; }
        public virtual Couple Couple { get; set; }
        public virtual WeddingPlanner? WeddingPlanner { get; set; }

        // --- THIS LINE IS FIXED ---
        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
        // --------------------------

        public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();
        public virtual ICollection<VendorBooking> VendorBookings { get; set; } = new List<VendorBooking>();
    }
}