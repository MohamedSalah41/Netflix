namespace Netflix_clone.Models;

public class Episode : MediaItem
{
    public int Number { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}
