using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers;

public class SeasonController : Controller
{
    private readonly ISeasonRepository _seasonRepo;

    public SeasonController(ISeasonRepository seasonRepo)
    {
        _seasonRepo = seasonRepo;
    }

    public IActionResult GetAllSeasons()
    {
        return View(_seasonRepo.GetAll());
    }

    public IActionResult GetSeasonById(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        return View(season);
    }

    public IActionResult AddSeason()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddSeason(Season season)
    {
        if (!ModelState.IsValid) return View(season);
        _seasonRepo.Add(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }

    public IActionResult UpdateSeason(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        return View(season);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateSeason(int id, Season season)
    {
        if (!ModelState.IsValid) return View(season);
        _seasonRepo.Update(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }

    public IActionResult DeleteSeason(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        return View(season);
    }

    [HttpPost, ActionName("DeleteSeason")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteSeasonConfirmed(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        _seasonRepo.Delete(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }
}
