using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers;

[Authorize]
public class MyListController : Controller
{
    private readonly NetflixContext _db;

    public MyListController(NetflixContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items  = _db.MyListItems
            .Where(m => m.AppUserId == userId)
            .OrderByDescending(m => m.AddedUtc)
            .ToList();
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Toggle(int mediaId, string mediaType, string mediaName, string? mediaPoster)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = _db.MyListItems
            .FirstOrDefault(m => m.AppUserId == userId && m.MediaId == mediaId && m.MediaType == mediaType);

        if (existing is not null)
        {
            _db.MyListItems.Remove(existing);
            _db.SaveChanges();
            return Json(new { inList = false });
        }

        _db.MyListItems.Add(new MyListItem
        {
            AppUserId   = userId,
            MediaId     = mediaId,
            MediaType   = mediaType,
            MediaName   = mediaName,
            MediaPoster = mediaPoster,
            AddedUtc    = DateTime.UtcNow
        });
        _db.SaveChanges();
        return Json(new { inList = true });
    }

    [HttpGet]
    public IActionResult Status(int mediaId, string mediaType)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var inList = _db.MyListItems
            .Any(m => m.AppUserId == userId && m.MediaId == mediaId && m.MediaType == mediaType);
        return Json(new { inList });
    }
}
