using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class Table {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public int Capacity { get; set; }
        public int WeddingId { get; set; }
        public virtual Wedding Wedding { get; set; }
        public virtual ICollection<Guest> SeatedGuests { get; set; } = new List<Guest>();
    }
}