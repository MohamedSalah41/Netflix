using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Netflix_clone.Models;
using Microsoft.EntityFrameworkCore;

namespace Netflix_clone.Controllers
{
	
	public class CategoryController : Controller
	{
		private readonly NetflixContext _context;
		public CategoryController(NetflixContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
			return View(categories);

		}

		public IActionResult Add()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(Category category)
		{
			if (!ModelState.IsValid)
				return View(category);

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Category added successfully.";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Update(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
				return NotFound();
			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id, Category category)
		{
			if (id != category.Id)
				return NotFound();

			if (!ModelState.IsValid)
				return View(category);

			_context.Categories.Update(category);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Category updated successfully.";
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Delete(int id)
		{
			var category = await _context.Categories.FindAsync(id);

			if (category == null)
				return NotFound();

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
				return NotFound();
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			TempData["Success"] = "Category deleted successfully.";
			return RedirectToAction(nameof(Index));
		}


	}
}