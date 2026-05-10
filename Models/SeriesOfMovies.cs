namespace Netflix_clone.Models;

public class SeriesOfMovies : GeneralSeries
{
    public ICollection<Movie>? Movies { get; set; } = new List<Movie>();

    // Own junction tables: SeriesOfMoviesActors, SeriesOfMoviesCategories
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
