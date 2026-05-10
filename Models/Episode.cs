using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_clone.Models;

public class Episode : MediaItem
{
    [Required(ErrorMessage = "This Field is Required")]
    public int Number { get; set; }
    [ForeignKey("Season")]
    public int? SeasonId { get; set; }
    public Season? Season { get; set; } = null!;
}
