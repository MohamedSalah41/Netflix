namespace Netflix_clone.Models
{
    public enum SubscriptionPlan
    {
        Basic,
        Standard,
        Premium
    }

    public enum SubscriptionStatus
    {
        Active,
        Cancelled,
        PastDue,
        Incomplete
    }

    public class Subscription
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
        public SubscriptionPlan Plan { get; set; }
        public SubscriptionStatus Status { get; set; }
        public string StripeCustomerId { get; set; } = string.Empty;
        public string StripeSubscriptionId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
