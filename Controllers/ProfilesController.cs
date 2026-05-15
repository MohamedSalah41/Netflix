using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IGenericRepository<Profile> _profileRepo;

        public ProfilesController(IGenericRepository<Profile> profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public async Task<IActionResult> SelectProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profiles = _profileRepo.FindAll(p => p.AppUserId == userId).ToList();
            return View(profiles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectProfile(int id)
        {
            HttpContext.Session.SetInt32("ActiveProfileId", id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ManageProfiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profiles = _profileRepo.FindAll(p => p.AppUserId == userId).ToList();
            return View(profiles);
        }

        public IActionResult AddProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfile(Profile profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            profile.AppUserId = userId;

            _profileRepo.Add(profile);
            await _profileRepo.SaveAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        public async Task<IActionResult> UpdateProfile(int id)
        {
            var profile = await _profileRepo.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, Profile profile)
        {
            if (id != profile.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(profile);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existing = await _profileRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (existing.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            existing.Name  = profile.Name;
            existing.Image = profile.Image;
            existing.IsKid = profile.IsKid;

            await _profileRepo.SaveAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _profileRepo.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(profile);
        }

        [HttpPost, ActionName("DeleteProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileConfirmed(int id)
        {
            var profile = await _profileRepo.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var activeId = HttpContext.Session.GetInt32("ActiveProfileId");
            if (activeId == id)
                HttpContext.Session.Remove("ActiveProfileId");

            _profileRepo.Delete(profile);
            await _profileRepo.SaveAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = _profileRepo.Find(p => p.Id == id);

            if (profile == null)
                return NotFound();

            return Ok(new { profile.Id, profile.Name, profile.Image, profile.IsKid, profile.AppUserId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = _profileRepo.GetAllWithIncludes(p => p.AppUser).ToList();
            return View(profiles);
        }
    }
}
