using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly IGenericRepository<Episode> _episodeRepo;
        private readonly IGenericRepository<Season> _seasonRepo;

        public EpisodeController(IGenericRepository<Episode> episodeRepo, IGenericRepository<Season> seasonRepo)
        {
            _episodeRepo = episodeRepo;
            _seasonRepo = seasonRepo;
        }
        public ActionResult GetAllEpisodes(int? seasonId = null)
        {
            if (seasonId.HasValue)
            {
                var episodes = _episodeRepo.GetAllQueryable()
                    .Where(e => e.SeasonId == seasonId.Value)
                    .OrderBy(e => e.Number)
                    .ToList();

                var season = _seasonRepo.GetByIdWithIncludes(seasonId.Value, s => s.Series);

                ViewBag.Season = season;
                return View(episodes);
            }

            return View(_episodeRepo.GetAll());
        }

        public ActionResult Details(int id)
        {
            return View(_episodeRepo.GetById(id));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Seasons = _seasonRepo.GetAll();
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
                    _episodeRepo.Add(episode);
                    _episodeRepo.Save();
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
            ViewBag.Seasons = _seasonRepo.GetAll();
            return View(_episodeRepo.GetById(id));
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
                   _episodeRepo.Update(episode);
                    _episodeRepo.Save();
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
            return View(_episodeRepo.GetById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Episode episode)
        {
            try
            {
                _episodeRepo.Delete(episode);
                _episodeRepo.Save();
                return RedirectToAction(nameof(GetAllEpisodes));
            }
            catch
            {
                return View(episode);
            }
        }
    }
}
