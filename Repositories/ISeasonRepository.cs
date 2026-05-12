using Netflix_clone.Models;

namespace Netflix_clone.Repositories;

public interface ISeasonRepository : IGenericRepository<Season>
{
    IEnumerable<Season> GetBySeriesId(int seriesId);
}
