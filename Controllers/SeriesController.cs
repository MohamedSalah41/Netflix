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
        // GET: SeriesController
        public ActionResult GetAllSeries()
        {
            return View(_netflixContext.Series.ToList());
        }

        // GET: SeriesController/Details/5


    public ActionResult GetSeriesById(int id)
        {
            var series = _netflixContext.Series
                .Include(s => s.Seasons)
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

        // POST: SeriesController/Create
        [HttpPost]
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

        // GET: SeriesController/Edit/5
        public ActionResult UpdateSeries(int id)
        {
            var series = _netflixContext.Series
                .Include(s => s.Actors)
                .Include(s => s.Categories)
                .FirstOrDefault(s => s.Id == id);

            if (series == null)
                return NotFound();

            ViewBag.Actors = _netflixContext.Actors.ToList();
            ViewBag.Categories = _netflixContext.Categories.ToList();

            return View(series);
        }

        // POST: SeriesController/Edit/5
        [HttpPost]
        public ActionResult UpdateSeries(
     Series series,
     List<int> actorIds,
     List<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Actors = _netflixContext.Actors.ToList();
                ViewBag.Categories = _netflixContext.Categories.ToList();
                return View(series);
            }

            var existingSeries = _netflixContext.Series
                .Include(s => s.Actors)
                .Include(s => s.Categories)
                .FirstOrDefault(s => s.Id == series.Id);

            if (existingSeries == null)
                return NotFound();

            // Update fields
            existingSeries.Name = series.Name;
            existingSeries.Description = series.Description;
            existingSeries.Poster = series.Poster;
            existingSeries.TrailerUrl = series.TrailerUrl;
            existingSeries.Rating = series.Rating;

            // Clear old relations
            existingSeries.Actors.Clear();
            existingSeries.Categories.Clear();

            // Add new Actors
            var actors = _netflixContext.Actors
                .Where(a => actorIds.Contains(a.Id))
                .ToList();

            var categories = _netflixContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToList();

            existingSeries.Actors = actors;
            existingSeries.Categories = categories;

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
