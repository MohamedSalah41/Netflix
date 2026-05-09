namespace Netflix_clone.Models;

public abstract class BaseItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public decimal Rating { get; set; }  // decimal(4,1) — supports up to 10.0
}
