using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    public class ActorController : Controller
    {
        private readonly NetflixContext _netflixContext;

        public ActorController(NetflixContext netflixContext)
        {
            _netflixContext = netflixContext;
        }
        // GET: ActorController
        public ActionResult GetActorByName(string name)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Name == name));
        }

        // GET: ActorController/Details/5
        public ActionResult GetActorByID(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        public ActionResult GetAllActors()
        {
            return View(_netflixContext.Actors.ToList());
        }

        // GET: ActorController/Create
        public ActionResult AddActor()
        {
            return View();
        }

        // POST: ActorController/Create
        [HttpPost]
        public ActionResult AddActor(Actor actor)
        {
            try
            {
                _netflixContext.Actors.Add(actor);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllActors));
            }
            catch
            {
                return View(actor);
            }
        }

        // GET: ActorController/Edit/5
        public ActionResult UpdateActor(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        // POST: ActorController/Edit/5
        [HttpPost]
        public ActionResult UpdateActor(int id, Actor actor)
        {
            try
            {
                _netflixContext.Actors.Update(actor);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllActors));
            }
            catch
            {
                return View(actor);
            }
        }

        // GET: ActorController/Delete/5
        public ActionResult DeleteActor(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        // POST: ActorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteActor(int id, Actor actor)
        {
            try
            {
                _netflixContext.Actors.Remove(actor);
                _netflixContext.SaveChanges();
                return RedirectToAction(nameof(GetAllActors));
            }
            catch
            {
                return View(actor);
            }
        }
    }
}
