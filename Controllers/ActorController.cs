using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class ActorController : Controller
    {
        private readonly IGenericRepository<Actor> _actorRepo;

        public ActorController(IGenericRepository<Actor> actorRepo)
        {
            _actorRepo = actorRepo;
        }

        [AllowAnonymous]
        public ActionResult GetActorByName(string name)
        {
            return View(_actorRepo.Find(a => a.Name == name));
        }

        [AllowAnonymous]
        public ActionResult GetActorByID(int id)
        {
            return View(_actorRepo.GetById(id));
        }

        [AllowAnonymous]
        public ActionResult GetAllActors()
        {
            return View(_actorRepo.GetAll());
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
                _actorRepo.Add(actor);
                _actorRepo.Save();
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
            return View(_actorRepo.GetById(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateActor(int id, Actor actor)
        {
            try
            {
                _actorRepo.Update(actor);
                _actorRepo.Save();
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
            return View(_actorRepo.GetById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteActor(int id, Actor actor)
        {
            try
            {
                _actorRepo.Delete(actor);
                _actorRepo.Save();
                return RedirectToAction(nameof(GetAllActors));
            }
            catch
            {
                return View(actor);
            }
        }
    }
}
