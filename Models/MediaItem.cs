using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public abstract class MediaItem : BaseItem
{
    [Required(ErrorMessage = "This Field is Required")]
    public string VideoUrl { get; set; } = string.Empty;
    public TimeSpan DurationSeconds { get; set; }
}
