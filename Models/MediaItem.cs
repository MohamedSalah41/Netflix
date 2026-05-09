namespace Netflix_clone.Models;

public abstract class MediaItem : BaseItem
{
    public string VideoUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}
