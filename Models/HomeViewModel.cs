namespace Netflix_clone.Models;

public class HomeViewModel
{
    public BaseItem? HeroItem { get; set; }
    public string HeroItemType { get; set; } = string.Empty;
    public string HeroGenres { get; set; } = string.Empty;
    public int HeroYear { get; set; }
    public string HeroDescription { get; set; } = string.Empty;

    public IEnumerable<MediaRow> ForYouItems { get; set; } = new List<MediaRow>();
    public IEnumerable<ContinueWatchingItem> ContinueWatching { get; set; } = new List<ContinueWatchingItem>();
    public IEnumerable<MediaRow> TrendingNow { get; set; } = new List<MediaRow>();
    public IEnumerable<MediaRow> NewReleases { get; set; } = new List<MediaRow>();
}

public class MediaRow
{
    public BaseItem Item { get; set; } = null!;
    public string ItemType { get; set; } = string.Empty;
}

public class ContinueWatchingItem
{
    public BaseItem Item { get; set; } = null!;
    public string ItemType { get; set; } = string.Empty;
    public double ProgressPercent { get; set; }
    public string LastWatched { get; set; } = string.Empty;
}
