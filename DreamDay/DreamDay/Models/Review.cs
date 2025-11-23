using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Review {
        [Key] public int Id { get; set; }
        [Range(1, 5)] public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
        public int VendorId { get; set; }
        public int CoupleId { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Couple Couple { get; set; }
    }
}