namespace Netflix_clone.Models;

public class Series : GeneralSeries
{
    // TODO: wire Season collection after Task 8
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}
