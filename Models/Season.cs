namespace Netflix_clone.Models;

public class Season
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int Number { get; set; }
    public string? Poster { get; set; }
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}
