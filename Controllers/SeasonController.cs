using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers;

public class SeasonController : Controller
{
    private readonly ISeasonRepository _seasonRepo;
    private readonly NetflixContext context;

    public SeasonController(ISeasonRepository seasonRepo,NetflixContext context)
    {
        _seasonRepo = seasonRepo;
        this.context = context;
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

    [Authorize(Roles = "Admin")]
    public IActionResult AddSeason()
    {
        ViewBag.SeriesList = context.Series.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult AddSeason(Season season)
    {
        if (!ModelState.IsValid) return View(season);
        ViewBag.SeriesList = context.Series.ToList();
        _seasonRepo.Add(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult UpdateSeason(int id)
    {
        var season = _seasonRepo.GetById(id);
        ViewBag.SeriesList = context.Series.ToList();
        if (season is null) return NotFound();
        return View(season);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateSeason(int id, Season season)
    {
        if (!ModelState.IsValid) return View(season);
        ViewBag.SeriesList = context.Series.ToList();
        _seasonRepo.Update(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult DeleteSeason(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        return View(season);
    }

    [HttpPost, ActionName("DeleteSeason")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteSeasonConfirmed(int id)
    {
        var season = _seasonRepo.GetById(id);
        if (season is null) return NotFound();
        _seasonRepo.Delete(season);
        _seasonRepo.Save();
        return RedirectToAction(nameof(GetAllSeasons));
    }
}
