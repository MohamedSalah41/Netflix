using Netflix_clone.Models;

namespace Netflix_clone.ViewModels;

public class CategoryBrowseViewModel
{
    public string CategoryName { get; set; } = string.Empty;
    public IList<Movie> Movies { get; set; } = new List<Movie>();
    public IList<Series> Series { get; set; } = new List<Series>();
}
