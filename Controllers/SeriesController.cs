using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class SeriesController : Controller
    {
        private readonly IGenericRepository<Series> _seriesRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Actor> _actorRepo;
        private readonly IGenericRepository<Profile> _profileRepo;

        public SeriesController(
            IGenericRepository<Series> seriesRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Actor> actorRepo,
            IGenericRepository<Profile> profileRepo)
        {
            _seriesRepo = seriesRepo;
            _categoryRepo = categoryRepo;
            _actorRepo = actorRepo;
            _profileRepo = profileRepo;
        }
        public ActionResult GetAllSeries()
        {
            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");
            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var profile = _profileRepo.GetById(activeProfileId.Value);
                isKid = profile?.IsKid ?? false;
            }

            var series = _seriesRepo.GetAllWithIncludes(s => s.Categories).ToList();

            if (isKid)
                series = series.Where(s => !s.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();

            return View(series);
        }

    public ActionResult GetSeriesById(int id)
        {
            var series = _seriesRepo.GetByIdWithIncludes(id, 
                s => s.Seasons, 
                s => s.Actors, 
                s => s.Categories);

            return View(series);
        }

    public ActionResult AddSeries()
        {
            ViewBag.Categories = _categoryRepo.GetAll();
            ViewBag.Actors = _actorRepo.GetAll();
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
                    var actors = _actorRepo.FindAll(a => actorIds.Contains(a.Id)).ToList();
                    series.Actors = actors;

                    var categories = _categoryRepo.FindAll(c => categoryIds.Contains(c.Id)).ToList();
                    series.Categories = categories;

                    series.Seasons = new List<Season>
            {
                new Season
                {
                    Name = "Season 1",
                    Number = 1,
                    Poster = series.Poster
                }
            };

                    _seriesRepo.Add(series);
                    _seriesRepo.Save();

                    return RedirectToAction(nameof(GetAllSeries));
                }

                ViewBag.Actors = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();

                return View(series);
            }
            catch
            {
                ViewBag.Actors = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();

                return View(series);
            }
        }

        public ActionResult UpdateSeries(int id)
        {
            var series = _seriesRepo.GetByIdWithIncludes(id, s => s.Actors, s => s.Categories);

            if (series == null)
                return NotFound();

            ViewBag.Actors              = _actorRepo.GetAll();
            ViewBag.Categories          = _categoryRepo.GetAll();
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
                ViewBag.Actors              = _actorRepo.GetAll();
                ViewBag.Categories          = _categoryRepo.GetAll();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(series);
            }

            var existingSeries = _seriesRepo.GetByIdWithIncludes(series.Id, s => s.Actors, s => s.Categories);

            if (existingSeries == null)
                return NotFound();

            existingSeries.Name        = series.Name;
            existingSeries.Description = series.Description;
            existingSeries.Poster      = series.Poster;
            existingSeries.TrailerUrl  = series.TrailerUrl;
            existingSeries.Rating      = series.Rating;

            existingSeries.Actors.Clear();
            existingSeries.Categories.Clear();

            existingSeries.Actors = _actorRepo.FindAll(a => actorIds.Contains(a.Id)).ToList();
            existingSeries.Categories = _categoryRepo.FindAll(c => categoryIds.Contains(c.Id)).ToList();

            _seriesRepo.Save();

            return RedirectToAction(nameof(GetAllSeries));
        }

        public ActionResult DeleteSeries(int id)
        {
            return View(_seriesRepo.GetById(id));
        }

        [HttpPost]
        public ActionResult DeleteSeries(int id, Series series)
        {
            try
            {
                _seriesRepo.Delete(series);
                _seriesRepo.Save();
                return RedirectToAction(nameof(GetAllSeries));
            }
            catch
            {
                return View();
            }
        }
    }
}
