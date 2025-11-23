using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public enum TaskCategory { VENUE, CATERING, DECORATION, PHOTOGRAPHY, ATTIRE, INVITATION, OTHER }
    public class ProjectTask {
        [Key] public int Id { get; set; }
        [Required] public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public TaskCategory Category { get; set; }
        public int WeddingId { get; set; }
        public virtual Wedding Wedding { get; set; }
    }
}