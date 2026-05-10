using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public class Actor
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50,ErrorMessage ="Max length is 50 Char")]
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Photo { get; set; }
}
