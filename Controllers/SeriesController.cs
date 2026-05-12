using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return View(_netflixContext.Series.FirstOrDefault(s => s.Id == id));
        }

        // GET: SeriesController/Create
        public ActionResult AddSeries()
        {
            return View();
        }

        // POST: SeriesController/Create
        [HttpPost]
        public ActionResult AddSeries(Series series)
        {
            try
            {
                _netflixContext.Series.Add(series);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllSeries));
            }
            catch
            {
                return View(series);
            }
        }

        // GET: SeriesController/Edit/5
        public ActionResult UpdateSeries(int id)
        {
            return View(_netflixContext.Series.FirstOrDefault(s => s.Id == id));
        }

        // POST: SeriesController/Edit/5
        [HttpPost]
        public ActionResult UpdateSeries(int id, Series series)
        {
            try
            {
                _netflixContext.Series.Update(series);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllSeries));
            }
            catch
            {
                return View(series);
            }
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
