using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        public string? AppUserId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; } = null!;
        [Required(ErrorMessage = "This Field is Required"), MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool IsKid { get; set; }=false;
        public ICollection<WatchHistory>? WatchHistory { get; set; } = new List<WatchHistory>();
    }
}
