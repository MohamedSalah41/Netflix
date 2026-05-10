namespace Netflix_clone.Models;

public class Movie : MediaItem
{
    public int? SeriesOfMoviesId { get; set; }
    public SeriesOfMovies? SeriesOfMovies { get; set; }

    // Junction tables: MovieActors, MovieCategories
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
