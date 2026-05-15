namespace Netflix_clone.Models;

public class SearchResultsViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<Movie> Movies { get; set; } = new List<Movie>();
    public List<Series> Series { get; set; } = new List<Series>();
    public List<Episode> Episodes { get; set; } = new List<Episode>();
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Actor> Actors { get; set; } = new List<Actor>();
    public int TotalResults => Movies.Count + Series.Count + Episodes.Count + Categories.Count + Actors.Count;
}
