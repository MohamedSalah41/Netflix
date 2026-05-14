using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    public class MovieController : Controller
    {
        private readonly NetflixContext context;

        public MovieController(NetflixContext context)
        {
            this.context = context;
        }
        public ActionResult GetAllMovies()
        {
            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");
            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var profile = context.Profiles.Find(activeProfileId.Value);
                isKid = profile?.IsKid ?? false;
            }

            var movies = context.Movies.Include(m => m.Categories).ToList();

            if (isKid)
                movies = movies.Where(m => !m.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();

            return View(movies);
        }

        // GET: MovieController/Details/5
        public ActionResult Details(int id)
        {
            var movie = context.Movies
                .Include(m => m.Actors)
                .Include(m => m.Categories)
                .FirstOrDefault(m => m.Id == id);
            return View(movie);
        }

        public ActionResult AddMovie()
        {
            ViewBag.Actors     = context.Actors.ToList();
            ViewBag.Categories = context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMovie(Movie NewMovie, List<int> actorIds, List<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Actors     = context.Actors.ToList();
                ViewBag.Categories = context.Categories.ToList();
                return View(NewMovie);
            }

            try
            {
                NewMovie.Actors     = context.Actors.Where(a => actorIds.Contains(a.Id)).ToList();
                NewMovie.Categories = context.Categories.Where(c => categoryIds.Contains(c.Id)).ToList();
                context.Movies.Add(NewMovie);
                context.SaveChanges();
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                ViewBag.Actors     = context.Actors.ToList();
                ViewBag.Categories = context.Categories.ToList();
                return View(NewMovie);
            }
        }

        public ActionResult Edit(int id)
        {
            var movie = context.Movies
                .Include(m => m.Actors)
                .Include(m => m.Categories)
                .FirstOrDefault(m => m.Id == id);

            if (movie is null) return NotFound();

            ViewBag.Actors     = context.Actors.ToList();
            ViewBag.Categories = context.Categories.ToList();
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
                ViewBag.Actors     = context.Actors.ToList();
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(DBMovie);
            }

            try
            {
                var existing = context.Movies
                    .Include(m => m.Actors)
                    .Include(m => m.Categories)
                    .FirstOrDefault(m => m.Id == id);

                if (existing is null) return NotFound();

                existing.Name            = DBMovie.Name;
                existing.Description     = DBMovie.Description;
                existing.Poster          = DBMovie.Poster;
                existing.VideoUrl        = DBMovie.VideoUrl;
                existing.DurationSeconds = DBMovie.DurationSeconds;
                existing.Rating          = DBMovie.Rating;

                existing.Actors.Clear();
                existing.Categories.Clear();

                existing.Actors = context.Actors
                    .Where(a => actorIds.Contains(a.Id)).ToList();
                existing.Categories = context.Categories
                    .Where(c => categoryIds.Contains(c.Id)).ToList();

                context.SaveChanges();
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                ViewBag.Actors     = context.Actors.ToList();
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.SelectedActorIds    = actorIds;
                ViewBag.SelectedCategoryIds = categoryIds;
                return View(DBMovie);
            }
        }

        // GET: MovieController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(context.Movies.FirstOrDefault(m=>m.Id==id));
        }

        // POST: MovieController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Movie movie)
        {
            try
            {
                context.Movies.Remove(context.Movies.FirstOrDefault(m => m.Id == id));
                context.SaveChanges();
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                return View(movie);
            }
        }
    }
}
