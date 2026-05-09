namespace Netflix_clone.Models;

public class SeriesOfMovies : GeneralSeries
{
    // TODO: wire Movie collection after Task 7
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
