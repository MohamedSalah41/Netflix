using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers;

[Route("api/history")]
[ApiController]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IGenericRepository<WatchHistory> _historyRepo;

    public HistoryController(IGenericRepository<WatchHistory> historyRepo)
    {
        _historyRepo = historyRepo;
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] HistoryUpdateDto dto)
    {
        var profileId = HttpContext.Session.GetInt32("ActiveProfileId");
        if (profileId == null)
            return Ok(new { saved = false });

        var entry = _historyRepo.Find(w => w.ProfileId == profileId && w.MediaItemId == dto.ContentId);

        if (entry == null)
        {
            entry = new WatchHistory
            {
                ProfileId      = profileId,
                MediaItemId    = dto.ContentId,
                Progress       = TimeSpan.FromSeconds(dto.WatchedSeconds),
                LastWatchedUtc = DateTime.UtcNow,
                IsFinished     = false
            };
            _historyRepo.Add(entry);
        }
        else
        {
            entry.Progress       = TimeSpan.FromSeconds(dto.WatchedSeconds);
            entry.LastWatchedUtc = DateTime.UtcNow;
        }

        _historyRepo.Save();
        return Ok(new { saved = true });
    }

    [HttpGet("progress/{contentId}")]
    public IActionResult Progress(int contentId)
    {
        var profileId = HttpContext.Session.GetInt32("ActiveProfileId");
        if (profileId == null)
            return Ok(new { watchedSeconds = 0, isFinished = false });

        var entry = _historyRepo.Find(w => w.ProfileId == profileId && w.MediaItemId == contentId);

        if (entry == null)
            return Ok(new { watchedSeconds = 0, isFinished = false });

        return Ok(new
        {
            watchedSeconds = (int)entry.Progress.TotalSeconds,
            isFinished     = entry.IsFinished
        });
    }
}

public class HistoryUpdateDto
{
    public int ContentId      { get; set; }
    public int WatchedSeconds { get; set; }
}
