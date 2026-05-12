using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Repositories;

public class SeasonRepository : GenericRepository<Season>, ISeasonRepository
{
    public SeasonRepository(NetflixContext context) : base(context) { }

    public IEnumerable<Season> GetBySeriesId(int seriesId) =>
        _dbSet.Where(s => s.SeriesId == seriesId).ToList();
}
