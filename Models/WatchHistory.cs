using System.ComponentModel.DataAnnotations;

namespace Netflix_clone.Models
{
    public class WatchHistory
    {
        [Key]
        public int Id { get; set; }
        public int? ProfileId { get; set; }
        public Profile? Profile { get; set; }
        public int? MediaItemId { get; set; }
        // No navigation to MediaItem — TPC stores Movie and Episode in separate tables.
        // MediaItemId is resolved in the service layer to either Movies or Episodes table.
        public TimeSpan Progress { get; set; }
        public DateTime LastWatchedUtc { get; set; }
        public bool IsFinished { get; set; } = false;
    }
}
