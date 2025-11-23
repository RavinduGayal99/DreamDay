using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models
{
    public enum BookingStatus { PENDING, CONFIRMED, CANCELLED, COMPLETED }

    public class VendorBooking
    {
        [Key]
        public int Id { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.PENDING;

        public string? Notes { get; set; } // For logistics notes

        // Foreign Keys
        public int WeddingId { get; set; }
        public int VendorId { get; set; }

        // Navigation Properties
        public virtual Wedding Wedding { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}