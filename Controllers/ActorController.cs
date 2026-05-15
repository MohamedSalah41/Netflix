using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ActorController : Controller
    {
        private readonly NetflixContext _netflixContext;

        public ActorController(NetflixContext netflixContext)
        {
            _netflixContext = netflixContext;
        }
        // GET: ActorController
        [AllowAnonymous]
        public ActionResult GetActorByName(string name)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Name == name));
        }

        // GET: ActorController/Details/5
        [AllowAnonymous]
        public ActionResult GetActorByID(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        [AllowAnonymous]
        public ActionResult GetAllActors()
        {
            return View(_netflixContext.Actors.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddActor()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateActor(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteActor(int id)
        {
            return View(_netflixContext.Actors.FirstOrDefault(a => a.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
