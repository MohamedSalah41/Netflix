using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.ViewModels
{
    public class EditViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
