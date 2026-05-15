using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers;

[Authorize]
public class MyListController : Controller
{
    private readonly IGenericRepository<MyListItem> _myListRepo;

    public MyListController(IGenericRepository<MyListItem> myListRepo)
    {
        _myListRepo = myListRepo;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items  = _myListRepo.FindAll(m => m.AppUserId == userId)
            .OrderByDescending(m => m.AddedUtc)
            .ToList();
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Toggle(int mediaId, string mediaType, string mediaName, string? mediaPoster)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = _myListRepo.Find(m => m.AppUserId == userId && m.MediaId == mediaId && m.MediaType == mediaType);

        if (existing is not null)
        {
            _myListRepo.Delete(existing);
            _myListRepo.Save();
            return Json(new { inList = false });
        }

        _myListRepo.Add(new MyListItem
        {
            AppUserId   = userId,
            MediaId     = mediaId,
            MediaType   = mediaType,
            MediaName   = mediaName,
            MediaPoster = mediaPoster,
            AddedUtc    = DateTime.UtcNow
        });
        _myListRepo.Save();
        return Json(new { inList = true });
    }

    [HttpGet]
    public IActionResult Status(int mediaId, string mediaType)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var inList = _myListRepo.FindAll(m => m.AppUserId == userId && m.MediaId == mediaId && m.MediaType == mediaType).Any();
        return Json(new { inList });
    }
}
