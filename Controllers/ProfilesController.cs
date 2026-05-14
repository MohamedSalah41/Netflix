using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly NetflixContext _context;

        public ProfilesController(NetflixContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SelectProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profiles = await _context.Profiles
                .Where(p => p.AppUserId == userId)
                .ToListAsync();
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
            var profiles = await _context.Profiles
                .Where(p => p.AppUserId == userId)
                .ToListAsync();
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

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        public async Task<IActionResult> UpdateProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
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
            var existing = await _context.Profiles.FindAsync(id);
            if (existing == null) return NotFound();
            if (existing.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            existing.Name  = profile.Name;
            existing.Image = profile.Image;
            existing.IsKid = profile.IsKid;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
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
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.AppUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var activeId = HttpContext.Session.GetInt32("ActiveProfileId");
            if (activeId == id)
                HttpContext.Session.Remove("ActiveProfileId");

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageProfiles));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await _context.Profiles
                .Where(p => p.Id == id)
                .Select(p => new { p.Id, p.Name, p.Image, p.IsKid, p.AppUserId })
                .FirstOrDefaultAsync();

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _context.Profiles
                                         .Include(p => p.AppUser)
                                         .ToListAsync();
            return View(profiles);
        }
    }
}
