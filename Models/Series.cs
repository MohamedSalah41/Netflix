namespace Netflix_clone.Models;

public class Series : GeneralSeries
{
    public ICollection<Season>? Seasons { get; set; } = new List<Season>();

    // Junction tables: SeriesActors, SeriesCategories
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
