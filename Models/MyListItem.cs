using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models;

public class MyListItem
{
    [Key]
    public int Id { get; set; }

    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;

    public int MediaId { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string MediaName { get; set; } = string.Empty;
    public string? MediaPoster { get; set; }

    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}
