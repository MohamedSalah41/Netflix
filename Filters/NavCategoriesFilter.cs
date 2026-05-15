using Microsoft.AspNetCore.Mvc.Filters;
using Netflix_clone.Models;
using Microsoft.EntityFrameworkCore;

namespace Netflix_clone.Filters;

public class NavCategoriesFilter : IAsyncActionFilter
{
    private readonly NetflixContext _db;

    public NavCategoriesFilter(NetflixContext db)
    {
        _db = db;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        if (context.Controller is Microsoft.AspNetCore.Mvc.Controller controller)
        {
            controller.ViewBag.NavCategories = categories;
        }

        await next();
    }
}
