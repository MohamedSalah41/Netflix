using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public abstract class BaseItem
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage ="This Field is Required"),MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "This Field is Required"), MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public decimal Rating { get; set; }  // decimal(4,1) — supports up to 10.0
}
