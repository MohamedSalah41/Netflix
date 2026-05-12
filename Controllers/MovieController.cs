using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // GET: MovieController
        public ActionResult GetAllMovies()
        {
            return View(context.Movies.ToList());
        }

        // GET: MovieController/Details/5
        public ActionResult Details(int id)
        {
            
            return View(context.Movies.FirstOrDefault(m => m.Id == id));
        }

        // GET: MovieController/Create
        public ActionResult AddMovie()
        {
            return View();
        }

        // POST: MovieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMovie(Movie NewMovie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Movies.Add(NewMovie);
                    context.SaveChanges();
                }

                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                return View(NewMovie);
            }
        }

        // GET: MovieController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(context.Movies.FirstOrDefault(m=>m.Id==id));
        }

        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Movie DBMovie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Update(DBMovie);
                    context.SaveChanges();
                }
                return RedirectToAction(nameof(GetAllMovies));
            }
            catch
            {
                return View();
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
