using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly NetflixContext _context;

        public ProfilesController(NetflixContext context)
        {
            _context = context;
        }

        // C: AddProfile -> USER
        [Authorize]
        public IActionResult AddProfile()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfile(Profile profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            profile.AppUserId = userId;

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // R: GetAllProfiles -> Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _context.Profiles
                                         .Include(p => p.AppUser)
                                         .ToListAsync();
            return View(profiles);
        }

        // U: UpdateProfile -> USER
        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, Profile profile)
        {
            if (id != profile.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(profile);

            try
            {
                _context.Update(profile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Profiles.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction("Index", "Home");
        }

        // D: DeleteProfile -> ANY
        // Show confirmation
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
                return NotFound();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileConfirmed(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
                return NotFound();

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET profile by ID (returns JSON)
        [HttpGet]

        public async Task<IActionResult> GitProfileByID(int id)
        {
            var profile = await _context.Profiles
                .Where(p => p.Id == id)
                .Select(p => new { p.Id, p.Name, p.Image, p.IsKid, p.AppUserId })
                .FirstOrDefaultAsync();

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
    }
}
