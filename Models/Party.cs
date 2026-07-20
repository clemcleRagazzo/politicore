using System.ComponentModel.DataAnnotations;

namespace Politicore.Models
{
    public class Party
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        // Navigation property for many-to-many relation with User
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
