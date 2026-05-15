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
		public CategoryController(IGenericRepository<Category> categoryRepo)
		{
			_categoryRepo = categoryRepo;
		}

		public async Task<IActionResult> Index()
		{
			var categories = (await _categoryRepo.GetAllAsync()).OrderBy(c => c.Name).ToList();
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

			_categoryRepo.Add(category);
			await _categoryRepo.SaveAsync();

			TempData["Success"] = "Category added successfully.";
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(int id)
		{
			var category = await _categoryRepo.GetByIdAsync(id);
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

			_categoryRepo.Update(category);
			await _categoryRepo.SaveAsync();

			TempData["Success"] = "Category updated successfully.";
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var category = await _categoryRepo.GetByIdAsync(id);

			if (category == null)
				return NotFound();

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var category = await _categoryRepo.GetByIdAsync(id);
			if (category == null)
				return NotFound();
			_categoryRepo.Delete(category);
			await _categoryRepo.SaveAsync();
			TempData["Success"] = "Category deleted successfully.";
			return RedirectToAction(nameof(Index));
		}
	}
}
