using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    public class SeriesController : Controller
    {
        private readonly NetflixContext _netflixContext;
        public SeriesController(NetflixContext netflixContext)
        {
            _netflixContext = netflixContext;
        }
        public ActionResult GetAllSeries()
        {
            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");
            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var profile = _netflixContext.Profiles.Find(activeProfileId.Value);
                isKid = profile?.IsKid ?? false;
            }

            var series = _netflixContext.Series.Include(s => s.Categories).ToList();

            if (isKid)
                series = series.Where(s => !s.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();

            return View(series);
        }

        // GET: SeriesController/Details/5


    public ActionResult GetSeriesById(int id)
        {
            var series = _netflixContext.Series
                .Include(s => s.Seasons)
                    .ThenInclude(s => s.Episodes)
                .Include(s => s.Actors)
                .Include(s => s.Categories)
                .FirstOrDefault(s => s.Id == id);

            return View(series);
        }

    // GET: SeriesController/Create
    public ActionResult AddSeries()
        {
            ViewBag.Categories = _netflixContext.Categories.ToList();
            ViewBag.Actors = _netflixContext.Actors.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSeries(
    Series series,
    List<int> actorIds,
    List<int> categoryIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Actors
                    var actors = _netflixContext.Actors
                        .Where(a => actorIds.Contains(a.Id))
                        .ToList();

                    series.Actors = actors;

                    // Categories
                    var categories = _netflixContext.Categories
                        .Where(c => categoryIds.Contains(c.Id))
                        .ToList();

                    series.Categories = categories;

                    // Default Season
                    series.Seasons = new List<Season>
            {
                new Season
                {
                    Name = "Season 1",
                    Number = 1,
                    Poster = series.Poster
                }
            };

                    _netflixContext.Series.Add(series);

                    _netflixContext.SaveChanges();

                    return RedirectToAction(nameof(GetAllSeries));
                }

                ViewBag.Actors = _netflixContext.Actors.ToList();
                ViewBag.Categories = _netflixContext.Categories.ToList();

                return View(series);
            }
            catch
            {
                ViewBag.Actors = _netflixContext.Actors.ToList();
                ViewBag.Categories = _netflixContext.Categories.ToList();

                return View(series);
            }
        }

        public ActionResult UpdateSeries(int id)
        {
            var series = _netflixContext.Series
                .Include(s => s.Actors)
                .Include(s => s.Categories)
                .FirstOrDefault(s => s.Id == id);

            if (series == null)
                return NotFound();

            ViewBag.Actors              = _netflixContext.Actors.ToList();
            ViewBag.Categories          = _netflixContext.Categories.ToList();
            ViewBag.SelectedActorIds    = series.Actors.Select(a => a.Id).ToList();
            ViewBag.SelectedCategoryIds = series.Categories.Select(c => c.Id).ToList();

            return View(series);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSeries(
     Series series,
     List<int> actorIds,
     List<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Actors              = _netflixContext.Actors.ToList();
                ViewBag.Categories          = _netflixContext.Categories.ToList();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(series);
            }

            var existingSeries = _netflixContext.Series
                .Include(s => s.Actors)
                .Include(s => s.Categories)
                .FirstOrDefault(s => s.Id == series.Id);

            if (existingSeries == null)
                return NotFound();

            existingSeries.Name        = series.Name;
            existingSeries.Description = series.Description;
            existingSeries.Poster      = series.Poster;
            existingSeries.TrailerUrl  = series.TrailerUrl;
            existingSeries.Rating      = series.Rating;

            existingSeries.Actors.Clear();
            existingSeries.Categories.Clear();

            existingSeries.Actors = _netflixContext.Actors
                .Where(a => actorIds.Contains(a.Id)).ToList();
            existingSeries.Categories = _netflixContext.Categories
                .Where(c => categoryIds.Contains(c.Id)).ToList();

            _netflixContext.SaveChanges();

            return RedirectToAction(nameof(GetAllSeries));
        }

        // GET: SeriesController/Delete/5
        public ActionResult DeleteSeries(int id)
        {
            return View(_netflixContext.Series.FirstOrDefault(s => s.Id == id));
        }

        // POST: SeriesController/Delete/5
        [HttpPost]
        public ActionResult DeleteSeries(int id, Series series)
        {
            try
            {
                _netflixContext.Series.Remove(series);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllSeries));
            }
            catch
            {
                return View();
            }
        }
    }
}
