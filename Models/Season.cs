using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public class Season
{
    [Key]
    public int Id { get; set; }
    public int? SeriesId { get; set; }
    public Series? Series { get; set; } = null!;
    [Required(ErrorMessage = "This Field is Required"), MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "This Field is Required")]
    public int Number { get; set; }
    public string? Poster { get; set; }
    public ICollection<Episode>? Episodes { get; set; } = new List<Episode>();
}
