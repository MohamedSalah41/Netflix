using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;

namespace Netflix_clone.Controllers
{
    public class SearchController : Controller
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Series> _seriesRepo;
        private readonly IGenericRepository<Episode> _episodeRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Actor> _actorRepo;

        public SearchController(
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Series> seriesRepo,
            IGenericRepository<Episode> episodeRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Actor> actorRepo)
        {
            _movieRepo = movieRepo;
            _seriesRepo = seriesRepo;
            _episodeRepo = episodeRepo;
            _categoryRepo = categoryRepo;
            _actorRepo = actorRepo;
        }

        public IActionResult Index(string query)
        {
            var model = new SearchResultsViewModel();

            if (string.IsNullOrWhiteSpace(query))
            {
                return View(model);
            }

            if (query.Length > 100)
            {
                query = query.Substring(0, 100);
            }

            model.Query = query;
            var normalizedQuery = query.ToLower();

            model.Movies = _movieRepo.GetAll()
                .Where(m => m.Name.ToLower().Contains(normalizedQuery) || 
                           (m.Description != null && m.Description.ToLower().Contains(normalizedQuery)))
                .ToList();

            model.Series = _seriesRepo.GetAll()
                .Where(s => s.Name.ToLower().Contains(normalizedQuery) || 
                           (s.Description != null && s.Description.ToLower().Contains(normalizedQuery)))
                .ToList();

            model.Episodes = _episodeRepo.GetAll()
                .Where(e => e.Name.ToLower().Contains(normalizedQuery) || 
                           (e.Description != null && e.Description.ToLower().Contains(normalizedQuery)))
                .ToList();

            model.Categories = _categoryRepo.GetAll()
                .Where(c => c.Name.ToLower().Contains(normalizedQuery))
                .ToList();

            model.Actors = _actorRepo.GetAll()
                .Where(a => a.Name.ToLower().Contains(normalizedQuery))
                .ToList();

            return View(model);
        }
    }
}
