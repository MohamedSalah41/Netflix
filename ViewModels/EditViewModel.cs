using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.ViewModels
{
    public class EditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
