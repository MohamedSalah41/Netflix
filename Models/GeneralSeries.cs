using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public abstract class GeneralSeries : BaseItem
{
    [Required(ErrorMessage = "This Field is Required")]
    public string TrailerUrl { get; set; } = string.Empty;

    // Actors and Categories are declared on each concrete subtype (Series, SeriesOfMovies)
    // because TPC requires separate junction tables per concrete type.
    // Do NOT add them here — EF cannot configure many-to-many on abstract types with TPC.
}
