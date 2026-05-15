using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly NetflixContext context;

        public EpisodeController(NetflixContext context)
        {
            this.context = context;
        }
        public ActionResult GetAllEpisodes(int? seasonId = null)
        {
            if (seasonId.HasValue)
            {
                var episodes = context.Episodes
                    .Include(e => e.Season)
                    .Where(e => e.SeasonId == seasonId.Value)
                    .OrderBy(e => e.Number)
                    .ToList();

                var season = context.Seasons
                    .Include(s => s.Series)
                    .FirstOrDefault(s => s.Id == seasonId.Value);

                ViewBag.Season = season;
                return View(episodes);
            }

            return View(context.Episodes.ToList());
        }

        // GET: EpisodeController/Details/5
        public ActionResult Details(int id)
        {
            return View(context.Episodes.FirstOrDefault(e=>e.Id==id));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Seasons = context.Seasons.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Episode episode)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    context.Episodes.Add(episode);
                    context.SaveChanges();
                }
                return RedirectToAction(nameof(GetAllEpisodes));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Seasons = context.Seasons.ToList();
            return View(context.Episodes.FirstOrDefault(e => e.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Episode episode)
        {
            try
            {
                if(ModelState.IsValid)
                {
                   context.Episodes.Update(episode);
                    context.SaveChanges();
                }
                return RedirectToAction(nameof(GetAllEpisodes));
            }
            catch
            {
                return View(episode);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View(context.Episodes.FirstOrDefault(e=>e.Id==id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Episode episode)
        {
            try
            {
                context.Episodes.Remove(episode);
                context.SaveChanges();
                return RedirectToAction(nameof(GetAllEpisodes));
            }
            catch
            {
                return View(episode);
            }
        }
    }
}
