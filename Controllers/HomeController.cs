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

        private static readonly string[] AgeRatings = { "G", "PG", "PG-13", "TV-14", "TV-MA", "R" };
        private static readonly Random Rng = new();

        private static string GetAgeRating(BaseItem item)
        {
            return AgeRatings[Math.Abs(item.Id) % AgeRatings.Length];
        }

        private static string GetDuration(BaseItem item)
        {
            if (item is MediaItem mi && mi.DurationSeconds > TimeSpan.Zero)
            {
                var ts = mi.DurationSeconds;
                return ts.Hours > 0 ? $"{ts.Hours}h {ts.Minutes}m" : $"{ts.Minutes}m";
            }
            if (item is GeneralSeries)
                return "Series";
            return string.Empty;
        }

        private static int GetYear(BaseItem item)
        {
            return 2020 + (Math.Abs(item.Id) % 5);
        }

        private static int GetMatchPct(BaseItem item)
        {
            return 85 + (Math.Abs(item.Id * 7 + 13) % 15);
        }

        private static string GetGenres(BaseItem item)
        {
            if (item is Series s) return string.Join(" · ", s.Categories.Select(c => c.Name));
            if (item is Movie m) return string.Join(" · ", m.Categories.Select(c => c.Name));
            return string.Empty;
        }

        private static MediaRow ToRow(BaseItem item, string type)
        {
            return new MediaRow
            {
                Item           = item,
                ItemType       = type,
                MatchPercentage = GetMatchPct(item),
                AgeRating      = GetAgeRating(item),
                Duration       = GetDuration(item),
                Year           = GetYear(item),
                Genres         = GetGenres(item)
            };
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

            var allRows = allSeries.Select(s => ToRow(s, "Series"))
                          .Concat(allMovies.Select(m => ToRow(m, "Movie")))
                          .ToList();

            var heroRow = allRows.OrderByDescending(r => r.Item.Rating).FirstOrDefault();

            string heroGenres = heroRow != null ? GetGenres(heroRow.Item) : string.Empty;
            string heroItemType = heroRow?.ItemType ?? string.Empty;
            int heroYear = heroRow != null ? GetYear(heroRow.Item) : DateTime.Now.Year;
            string heroAgeRating = heroRow != null ? GetAgeRating(heroRow.Item) : string.Empty;
            string heroTrailerUrl = string.Empty;
            
            if (heroRow?.Item != null)
            {
                if (heroRow.Item is GeneralSeries gs)
                    heroTrailerUrl = gs.TrailerUrl ?? string.Empty;
                else if (heroRow.Item is MediaItem mi)
                    heroTrailerUrl = mi.TrailerUrl ?? string.Empty;
            }

            var forYou = allRows.OrderBy(_ => Guid.NewGuid()).Take(15).ToList();
            var trending = allRows.OrderByDescending(r => r.Item.Rating).Take(15).ToList();
            var newReleases = allRows.OrderBy(_ => Guid.NewGuid()).Take(15).ToList();

            var actionKeywords = new[] { "action", "thriller", "adventure" };
            var dramaKeywords  = new[] { "drama", "crime", "mystery" };

            var actionMovies = allRows
                .Where(r => r.Genres.Split('·', StringSplitOptions.TrimEntries)
                    .Any(g => actionKeywords.Any(k => g.Contains(k, StringComparison.OrdinalIgnoreCase))))
                .Take(15).ToList();

            if (actionMovies.Count < 5)
                actionMovies = allRows.OrderBy(_ => Guid.NewGuid()).Take(15).ToList();

            var tvDramas = allRows
                .Where(r => r.ItemType == "Series" || r.Genres.Split('·', StringSplitOptions.TrimEntries)
                    .Any(g => dramaKeywords.Any(k => g.Contains(k, StringComparison.OrdinalIgnoreCase))))
                .Take(15).ToList();

            if (tvDramas.Count < 5)
                tvDramas = allRows.Where(r => r.ItemType == "Series").Take(15).ToList();

            var popular = allRows.OrderByDescending(r => r.Item.Rating).Skip(5).Take(15).ToList();

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
                HeroItem              = heroRow?.Item,
                HeroItemType          = heroItemType,
                HeroGenres            = heroGenres,
                HeroYear              = heroYear,
                HeroDescription       = heroRow?.Item.Description ?? string.Empty,
                HeroAgeRating         = heroAgeRating,
                HeroTrailerUrl        = heroTrailerUrl,
                ForYouItems           = forYou,
                ContinueWatching      = continueWatching,
                TrendingNow           = trending,
                NewReleases           = newReleases,
                PopularOnStreamFlix   = popular,
                ActionMovies          = actionMovies,
                TvDramas              = tvDramas
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

        [HttpGet]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(new { results = new List<object>() });
            }

            var query = q.Trim().ToLower();

            var activeProfileId = HttpContext.Session.GetInt32("ActiveProfileId");
            bool isKid = false;
            if (activeProfileId.HasValue)
            {
                var activeProfile = _profileRepo.GetById(activeProfileId.Value);
                isKid = activeProfile?.IsKid ?? false;
            }

            var movies = _movieRepo.GetAllWithIncludes(m => m.Categories)
                .Where(m => (!string.IsNullOrEmpty(m.Name) && m.Name.ToLower().Contains(query)) || (!string.IsNullOrEmpty(m.Description) && m.Description.ToLower().Contains(query)))
                .Take(10)
                .ToList();

            var series = _seriesRepo.GetAllWithIncludes(s => s.Categories)
                .Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(query)) || (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(query)))
                .Take(10)
                .ToList();

            if (isKid)
            {
                movies = movies.Where(m => !m.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
                series = series.Where(s => !s.Categories.Any(c => c.Name.Equals("18+", StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var results = movies.Select(m => new
            {
                id = m.Id,
                name = m.Name,
                type = "Movie",
                poster = m.Poster,
                rating = m.Rating,
                year = GetYear(m),
                url = Url.Action("Details", "Movie", new { id = m.Id })
            })
            .Concat(series.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                type = "Series",
                poster = s.Poster,
                rating = s.Rating,
                year = GetYear(s),
                url = Url.Action("GetSeriesById", "Series", new { id = s.Id })
            }))
            .OrderByDescending(r => r.rating)
            .Take(8)
            .ToList();

            return Json(new { results });
        }
    }
}
