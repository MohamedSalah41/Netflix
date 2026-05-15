using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class MovieController : Controller
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Actor> _actorRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Profile> _profileRepo;

        public MovieController(
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Actor> actorRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Profile> profileRepo)
        {
            _movieRepo = movieRepo;
            _actorRepo = actorRepo;
            _categoryRepo = categoryRepo;
            _profileRepo = profileRepo;
        }
        public ActionResult GetAllMovies()
        {
            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");
            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var profile = _profileRepo.GetById(activeProfileId.Value);
                isKid = profile?.IsKid ?? false;
            }

            var movies = _movieRepo.GetAllWithIncludes(m => m.Categories).ToList();

            if (isKid)
                movies = movies.Where(m => !m.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();

            return View(movies);
        }

        public ActionResult Details(int id)
        {
            var movie = _movieRepo.GetByIdWithIncludes(id, m => m.Actors, m => m.Categories);
            return View(movie);
        }

        public ActionResult AddMovie()
        {
            ViewBag.Actors     = _actorRepo.GetAll();
            ViewBag.Categories = _categoryRepo.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMovie(Movie NewMovie, List<int> actorIds, List<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Actors     = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(NewMovie);
            }

            try
            {
                NewMovie.Actors     = _actorRepo.FindAll(a => actorIds.Contains(a.Id)).ToList();
                NewMovie.Categories = _categoryRepo.FindAll(c => categoryIds.Contains(c.Id)).ToList();
                _movieRepo.Add(NewMovie);
                _movieRepo.Save();
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                ViewBag.Actors     = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(NewMovie);
            }
        }

        public ActionResult Edit(int id)
        {
            var movie = _movieRepo.GetByIdWithIncludes(id, m => m.Actors, m => m.Categories);

            if (movie is null) return NotFound();

            ViewBag.Actors     = _actorRepo.GetAll();
            ViewBag.Categories = _categoryRepo.GetAll();
            ViewBag.SelectedActorIds    = movie.Actors.Select(a => a.Id).ToList();
            ViewBag.SelectedCategoryIds = movie.Categories.Select(c => c.Id).ToList();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Movie DBMovie, List<int> actorIds, List<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Actors     = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(DBMovie);
            }

            try
            {
                var existing = _movieRepo.GetByIdWithIncludes(id, m => m.Actors, m => m.Categories);

                if (existing is null) return NotFound();

                existing.Name            = DBMovie.Name;
                existing.Description     = DBMovie.Description;
                existing.Poster          = DBMovie.Poster;
                existing.VideoUrl        = DBMovie.VideoUrl;
                existing.DurationSeconds = DBMovie.DurationSeconds;
                existing.Rating          = DBMovie.Rating;

                existing.Actors.Clear();
                existing.Categories.Clear();

                existing.Actors = _actorRepo.FindAll(a => actorIds.Contains(a.Id)).ToList();
                existing.Categories = _categoryRepo.FindAll(c => categoryIds.Contains(c.Id)).ToList();

                _movieRepo.Save();
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                ViewBag.Actors     = _actorRepo.GetAll();
                ViewBag.Categories = _categoryRepo.GetAll();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(DBMovie);
            }
        }

        public ActionResult Delete(int id)
        {
            return View(_movieRepo.GetById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Movie movie)
        {
            try
            {
                var existing = _movieRepo.GetById(id);
                if (existing != null)
                {
                    _movieRepo.Delete(existing);
                    _movieRepo.Save();
                }
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                return View(movie);
            }
        }
    }
}
