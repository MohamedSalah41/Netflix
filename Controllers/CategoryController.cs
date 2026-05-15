using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;
using Netflix_clone.Repositories;
using Netflix_clone.ViewModels;

namespace Netflix_clone.Controllers
{
	public class CategoryController : Controller
	{
		private readonly IGenericRepository<Category> _categoryRepo;
		private readonly NetflixContext _db;

		public CategoryController(IGenericRepository<Category> categoryRepo, NetflixContext db)
		{
			_categoryRepo = categoryRepo;
			_db = db;
		}

		public async Task<IActionResult> Browse(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return NotFound();

			var category = await _db.Categories
				.FirstOrDefaultAsync(c => c.Name == name);

			if (category == null)
				return NotFound();

			var movies = await _db.Movies
				.Include(m => m.Categories)
				.Where(m => m.Categories.Any(c => c.Name == name))
				.OrderBy(m => m.Name)
				.ToListAsync();

			var series = await _db.Series
				.Include(s => s.Categories)
				.Where(s => s.Categories.Any(c => c.Name == name))
				.OrderBy(s => s.Name)
				.ToListAsync();

			var vm = new CategoryBrowseViewModel
			{
				CategoryName = category.Name,
				Movies = movies,
				Series = series
			};

			return View(vm);
		}

		public async Task<IActionResult> Index()
		{
			var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
			return View(categories);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Add(Category category)
		{
			if (!ModelState.IsValid)
				return View(category);

			_db.Categories.Add(category);
			await _db.SaveChangesAsync();

			TempData["Success"] = "Category added successfully.";
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(int id)
		{
			var category = await _db.Categories.FindAsync(id);
			if (category == null)
				return NotFound();
			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(int id, Category category)
		{
			if (id != category.Id)
				return NotFound();

			if (!ModelState.IsValid)
				return View(category);

			_db.Categories.Update(category);
			await _db.SaveChangesAsync();

			TempData["Success"] = "Category updated successfully.";
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var category = await _db.Categories.FindAsync(id);

			if (category == null)
				return NotFound();

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var category = await _db.Categories.FindAsync(id);
			if (category == null)
				return NotFound();
			_db.Categories.Remove(category);
			await _db.SaveChangesAsync();
			TempData["Success"] = "Category deleted successfully.";
			return RedirectToAction(nameof(Index));
		}
	}
}
