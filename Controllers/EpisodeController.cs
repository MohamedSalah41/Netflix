using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // GET: EpisodeController
        public ActionResult GetAllEpisodes()
        {
            return View(context.Episodes.ToList());
        }

        // GET: EpisodeController/Details/5
        public ActionResult Details(int id)
        {
            return View(context.Episodes.FirstOrDefault(e=>e.Id==id));
        }

        // GET: EpisodeController/Create
        public ActionResult Create()
        {
            ViewBag.Seasons = context.Seasons.ToList();
            return View();
        }

        // POST: EpisodeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: EpisodeController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Seasons = context.Seasons.ToList();
            return View(context.Episodes.FirstOrDefault(e => e.Id == id));
        }

        // POST: EpisodeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: EpisodeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(context.Episodes.FirstOrDefault(e=>e.Id==id));
        }

        // POST: EpisodeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
