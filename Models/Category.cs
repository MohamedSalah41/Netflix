using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage ="this Field Is Required"),MaxLength(20) ]
    public string Name { get; set; } = string.Empty;
}
