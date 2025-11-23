using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Vendor {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Category { get; set; }
        public string? ContactInfo { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<VendorService> Services { get; set; } = new List<VendorService>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<VendorBooking> Bookings { get; set; } = new List<VendorBooking>();
    }
}