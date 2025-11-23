using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace DreamDay.Models {
    public class User : IdentityUser<int> {
        [Required] public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}