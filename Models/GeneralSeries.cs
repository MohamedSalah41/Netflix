namespace Netflix_clone.Models;

public abstract class GeneralSeries : BaseItem
{
    public string TrailerUrl { get; set; } = string.Empty;
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
