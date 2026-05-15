using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Series> _seriesRepo;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Episode> _episodeRepo;
        private readonly IGenericRepository<Profile> _profileRepo;
        private readonly IGenericRepository<WatchHistory> _historyRepo;
        private readonly IGenericRepository<Subscription> _subscriptionRepo;

        public HomeController(
            IGenericRepository<Series> seriesRepo,
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Episode> episodeRepo,
            IGenericRepository<Profile> profileRepo,
            IGenericRepository<WatchHistory> historyRepo,
            IGenericRepository<Subscription> subscriptionRepo)
        {
            _seriesRepo = seriesRepo;
            _movieRepo = movieRepo;
            _episodeRepo = episodeRepo;
            _profileRepo = profileRepo;
            _historyRepo = historyRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasSub = _subscriptionRepo.FindAll(s => s.AppUserId == uid && s.Status == SubscriptionStatus.Active).Any();
                if (!hasSub)
                    return RedirectToAction("Plans", "Payment");
            }

            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");

            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var activeProfile = _profileRepo.GetById(activeProfileId.Value);
                isKid = activeProfile?.IsKid ?? false;
            }

            var allSeries = _seriesRepo.GetAllWithIncludes(s => s.Categories).ToList();
            var allMovies = _movieRepo.GetAllWithIncludes(m => m.Categories).ToList();

            if (isKid)
            {
                allSeries = allSeries.Where(s => !s.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
                allMovies = allMovies.Where(m => !m.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var allRows = allSeries.Select(s => new MediaRow { Item = s, ItemType = "Series" })
                          .Concat(allMovies.Select(m => new MediaRow { Item = m, ItemType = "Movie" }))
                          .ToList();

            var heroRow = allRows.OrderByDescending(r => r.Item.Rating).FirstOrDefault();

            string heroGenres = string.Empty;
            string heroItemType = heroRow?.ItemType ?? string.Empty;
            int heroYear = DateTime.Now.Year;

            if (heroRow?.Item is Series hs)
                heroGenres = string.Join(" . ", hs.Categories.Select(c => c.Name));
            else if (heroRow?.Item is Movie hm)
                heroGenres = string.Join(" . ", hm.Categories.Select(c => c.Name));

            var forYou = allRows.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
            var trending = allRows.OrderByDescending(r => r.Item.Rating).Take(10).ToList();
            var newReleases = allRows.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();

            var continueWatching = new List<ContinueWatchingItem>();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                IEnumerable<WatchHistory> histories;

                if (activeProfileId.HasValue)
                {
                    histories = _historyRepo.FindAll(w => w.ProfileId == activeProfileId.Value && !w.IsFinished)
                        .OrderByDescending(w => w.LastWatchedUtc)
                        .Take(10)
                        .ToList();
                }
                else
                {
                    var profileIds = _profileRepo.FindAll(p => p.AppUserId == userId)
                        .Select(p => p.Id)
                        .ToList();

                    histories = _historyRepo.FindAll(w => w.ProfileId != null && profileIds.Contains(w.ProfileId.Value) && !w.IsFinished)
                        .OrderByDescending(w => w.LastWatchedUtc)
                        .Take(10)
                        .ToList();
                }

                foreach (var h in histories)
                {
                    if (h.MediaItemId == null) continue;

                    Movie? movie = _movieRepo.GetById(h.MediaItemId.Value);
                    Episode? episode = movie == null ? _episodeRepo.GetById(h.MediaItemId.Value) : null;
                    BaseItem? item = (BaseItem?)movie ?? episode;

                    if (item == null) continue;

                    var totalSeconds = item is MediaItem mi ? mi.DurationSeconds.TotalSeconds : 0;
                    var pct = totalSeconds > 0
                        ? Math.Min(100, h.Progress.TotalSeconds / totalSeconds * 100)
                        : 0;

                    continueWatching.Add(new ContinueWatchingItem
                    {
                        Item            = item,
                        ItemType        = movie != null ? "Movie" : "Episode",
                        ProgressPercent = pct,
                        LastWatched     = h.LastWatchedUtc.ToString("MMM d")
                    });
                }

                var userProfiles = _profileRepo.FindAll(p => p.AppUserId == userId).ToList();

                ViewBag.UserProfiles    = userProfiles;
                ViewBag.ActiveProfileId = activeProfileId;
            }

            var vm = new HomeViewModel
            {
                HeroItem         = heroRow?.Item,
                HeroItemType     = heroItemType,
                HeroGenres       = heroGenres,
                HeroYear         = heroYear,
                HeroDescription  = heroRow?.Item.Description ?? string.Empty,
                ForYouItems      = forYou,
                ContinueWatching = continueWatching,
                TrendingNow      = trending,
                NewReleases      = newReleases
            };

            return View(vm);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
                return View("NotFound");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Browse(string category)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasSub = _subscriptionRepo.FindAll(s => s.AppUserId == uid && s.Status == SubscriptionStatus.Active).Any();
                if (!hasSub)
                    return RedirectToAction("Plans", "Payment");
            }

            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");

            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var activeProfile = _profileRepo.GetById(activeProfileId.Value);
                isKid = activeProfile?.IsKid ?? false;
            }

            var allSeries = _seriesRepo.GetAllWithIncludes(s => s.Categories).ToList();
            var allMovies = _movieRepo.GetAllWithIncludes(m => m.Categories).ToList();

            if (isKid)
            {
                allSeries = allSeries.Where(s => !s.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
                allMovies = allMovies.Where(m => !m.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                allSeries = allSeries.Where(s => s.Categories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase))).ToList();
                allMovies = allMovies.Where(m => m.Categories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var allRows = allSeries.Select(s => new MediaRow { Item = s, ItemType = "Series" })
                          .Concat(allMovies.Select(m => new MediaRow { Item = m, ItemType = "Movie" }))
                          .ToList();

            ViewBag.CategoryName = string.IsNullOrEmpty(category) ? "All Content" : category;
            ViewBag.TotalCount = allRows.Count;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userProfiles = _profileRepo.FindAll(p => p.AppUserId == userId).ToList();
                ViewBag.UserProfiles = userProfiles;
                ViewBag.ActiveProfileId = activeProfileId;
            }

            return View(allRows);
        }
    }
}
