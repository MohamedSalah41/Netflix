using Microsoft.AspNetCore.Identity;

namespace Netflix_clone.Models
{
    public class AppUser:IdentityUser
    {
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    }
}
