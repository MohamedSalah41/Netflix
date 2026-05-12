# Category Controller — Implementation Breakdown

> **Admin-only CRUD** for the `Category` model.  
> All actions are protected with `[Authorize(Roles = "Admin")]`.

---

## Project Structure

```
Controllers/
└── CategoryController.cs

Views/
└── Category/
    ├── Index.cshtml        ← GetAllCategories view
    ├── Add.cshtml          ← AddCategory form
    ├── Update.cshtml       ← UpdateCategory form
    └── Delete.cshtml       ← DeleteCategory confirmation
```

---

## Step 1 — Controller Skeleton

Create `Controllers/CategoryController.cs` and inject the `NetflixContext`.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly NetflixContext _context;

        public CategoryController(NetflixContext context)
        {
            _context = context;
        }
    }
}
```

---

## Step 2 — R: GetAllCategories (Index)

Fetches every category from the DB and passes the list to the view.

```csharp
// GET: /Category
public async Task<IActionResult> Index()
{
    var categories = await _context.Categories
        .OrderBy(c => c.Name)
        .ToListAsync();

    return View(categories);
}
```

**View — `Views/Category/Index.cshtml`**

```html
@model IEnumerable<Netflix_clone.Models.Category>

<h2>All Categories</h2>

<a asp-action="Add" class="btn btn-primary mb-3">+ Add Category</a>

<table class="table">
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var cat in Model)
        {
            <tr>
                <td>@cat.Id</td>
                <td>@cat.Name</td>
                <td>
                    <a asp-action="Update" asp-route-id="@cat.Id" class="btn btn-sm btn-warning">Edit</a>
                    <a asp-action="Delete" asp-route-id="@cat.Id" class="btn btn-sm btn-danger">Delete</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

## Step 3 — C: AddCategory

Two actions — `GET` renders the empty form, `POST` validates and saves.

```csharp
// GET: /Category/Add
public IActionResult Add()
{
    return View();
}

// POST: /Category/Add
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
```

**View — `Views/Category/Add.cshtml`**

```html
@model Netflix_clone.Models.Category

<h2>Add Category</h2>

<form asp-action="Add" method="post">
    @Html.AntiForgeryToken()

    <div class="mb-3">
        <label asp-for="Name" class="form-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-success">Save</button>
    <a asp-action="Index" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
}
```

---

## Step 4 — U: UpdateCategory

`GET` loads the existing category by ID, `POST` applies the changes.

```csharp
// GET: /Category/Update/5
public async Task<IActionResult> Update(int id)
{
    var category = await _context.Categories.FindAsync(id);

    if (category == null)
        return NotFound();

    return View(category);
}

// POST: /Category/Update/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(int id, Category category)
{
    if (id != category.Id)
        return BadRequest();

    if (!ModelState.IsValid)
        return View(category);

    _context.Categories.Update(category);
    await _context.SaveChangesAsync();

    TempData["Success"] = "Category updated successfully.";
    return RedirectToAction(nameof(Index));
}
```

**View — `Views/Category/Update.cshtml`**

```html
@model Netflix_clone.Models.Category

<h2>Edit Category</h2>

<form asp-action="Update" asp-route-id="@Model.Id" method="post">
    @Html.AntiForgeryToken()

    <input type="hidden" asp-for="Id" />

    <div class="mb-3">
        <label asp-for="Name" class="form-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-warning">Update</button>
    <a asp-action="Index" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
}
```

---

## Step 5 — D: DeleteCategory

`GET` shows a confirmation page with the category name, `POST` performs the delete.

```csharp
// GET: /Category/Delete/5
public async Task<IActionResult> Delete(int id)
{
    var category = await _context.Categories.FindAsync(id);

    if (category == null)
        return NotFound();

    return View(category);
}

// POST: /Category/Delete/5
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
```

**View — `Views/Category/Delete.cshtml`**

```html
@model Netflix_clone.Models.Category

<h2>Delete Category</h2>

<div class="alert alert-danger">
    Are you sure you want to delete <strong>@Model.Name</strong>?
</div>

<form asp-action="Delete" asp-route-id="@Model.Id" method="post">
    @Html.AntiForgeryToken()
    <button type="submit" class="btn btn-danger">Yes, Delete</button>
    <a asp-action="Index" class="btn btn-secondary">Cancel</a>
</form>
```

---

## Full Controller (Final)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

namespace Netflix_clone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly NetflixContext _context;

        public CategoryController(NetflixContext context)
        {
            _context = context;
        }

        // ── R: Get All ────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // ── C: Add ────────────────────────────────────────────────────────
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

        // ── U: Update ─────────────────────────────────────────────────────
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
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ── D: Delete ─────────────────────────────────────────────────────
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
```

---

## Notes — What's Missing / Should Be Added

### 1. Duplicate Name Check
The `Category` model has no unique constraint on `Name` in the DB, so you can create two categories both named "Action". Add a check before saving:

```csharp
// Inside Add POST, before SaveChangesAsync:
bool exists = await _context.Categories
    .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

if (exists)
{
    ModelState.AddModelError("Name", "A category with this name already exists.");
    return View(category);
}
```

Apply the same check in the `Update POST`, but exclude the current record:

```csharp
bool exists = await _context.Categories
    .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != id);
```

---

### 2. Delete Cascade Safety Check
`Category` is used in junction tables (`SeriesCategories`, `SeriesOfMoviesCategories`, `MovieCategories`). Deleting a category that is still assigned to content will either throw a FK violation or silently orphan rows depending on the DB cascade setting.

Add a usage check before deleting:

```csharp
// Inside DeleteConfirmed, before Remove:
bool isInUse = await _context.Series.AnyAsync(s => s.Categories.Any(c => c.Id == id))
            || await _context.SeriesOfMovies.AnyAsync(s => s.Categories.Any(c => c.Id == id))
            || await _context.Movies.AnyAsync(m => m.Categories.Any(c => c.Id == id));

if (isInUse)
{
    TempData["Error"] = "Cannot delete — this category is assigned to existing content.";
    return RedirectToAction(nameof(Index));
}
```

---

### 3. TempData Feedback in Layout / Index View
`TempData["Success"]` and `TempData["Error"]` are set but never displayed unless you add this to `_Layout.cshtml` or the `Index` view:

```html
@if (TempData["Success"] != null)
{
    <div class="alert alert-success">@TempData["Success"]</div>
}
@if (TempData["Error"] != null)
{
    <div class="alert alert-danger">@TempData["Error"]</div>
}
```

---

### 4. Unique DB Constraint (Migration)
To enforce uniqueness at the database level (not just in code), add this to `NetflixContext.OnModelCreating`:

```csharp
modelBuilder.Entity<Category>(entity =>
{
    entity.HasIndex(c => c.Name).IsUnique();
});
```

Then run:
```
dotnet ef migrations add AddCategoryNameUniqueIndex
dotnet ef database update
```

---

### 5. Pagination on Index (Optional but Recommended)
If categories grow large, the `Index` view will get unwieldy. A simple page size of 20 with skip/take is enough:

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
{
    var categories = await _context.Categories
        .OrderBy(c => c.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return View(categories);
}
```

---

---

## Cross-Controller Compatibility Check

Compared `CategoryController` against `ActorController`, `ProfilesController`, and `HomeController`.

---

### ✅ What CategoryController Gets Right

| Practice | CategoryController | Verdict |
|---|---|---|
| `async/await` + `Async` DB methods | ✅ | Correct |
| `[ValidateAntiForgeryToken]` on all POSTs | ✅ | Correct |
| `ModelState.IsValid` check before saving | ✅ | Correct |
| Fetch entity from DB before Remove | ✅ | Correct |
| `[Authorize(Roles = "Admin")]` on class | ✅ | Correct |
| `TempData` for user feedback | ✅ | Correct |
| `_context` field name | ✅ matches Profiles | Consistent |

---

### ⚠️ WARNING 1 — `Update` POST id-mismatch returns `NotFound()` instead of `BadRequest()`

**In the actual `CategoryController.cs` file:**
```csharp
if (id != category.Id)
    return NotFound();   // ← wrong HTTP status for a mismatch
```

**In `ProfilesController.cs` (correct pattern):**
```csharp
if (id != profile.Id)
    return BadRequest();  // ← correct, the request itself is malformed
```

**In the md breakdown doc (Step 4), the code says `BadRequest()` — but the actual file says `NotFound()`.** The doc and the real file are out of sync. `NotFound` (404) is semantically wrong here — the record exists, the submitted ID just doesn't match the route ID. Fix the actual controller:

```csharp
// CategoryController.cs — Update POST
if (id != category.Id)
    return BadRequest();  // ← change this
```

---

### ⚠️ WARNING 2 — `ActorController` has zero `[Authorize]` — admin surface is half-open

`CategoryController` locks everything behind `[Authorize(Roles = "Admin")]`. `ActorController` has **no authorization at all** — any anonymous visitor can add, edit, or delete actors. These two admin controllers are completely inconsistent in their security posture.

**Fix needed in `ActorController`:**
```csharp
[Authorize(Roles = "Admin")]
public class ActorController : Controller { ... }
```

---

### ⚠️ WARNING 3 — `ActorController` uses synchronous DB calls everywhere

`CategoryController` uses `async/await` with `FindAsync`, `ToListAsync`, `SaveChangesAsync`. `ActorController` uses `FirstOrDefault`, `ToList`, `SaveChanges` — all blocking. Under any real load this will starve the thread pool.

**Every action in `ActorController` needs to be converted**, for example:

```csharp
// Before (blocking)
public ActionResult GetAllActors()
{
    return View(_netflixContext.Actors.ToList());
}

// After (non-blocking)
public async Task<IActionResult> GetAllActors()
{
    return View(await _netflixContext.Actors.ToListAsync());
}
```

---

### ⚠️ WARNING 4 — `ActorController` missing `[ValidateAntiForgeryToken]` on `AddActor` and `UpdateActor` POST

Only `DeleteActor` POST has the token. `AddActor` and `UpdateActor` POST actions are unprotected against CSRF. `CategoryController` has it on all three POSTs — correct.

**Fix in `ActorController`:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]   // ← add this to AddActor and UpdateActor POST
public ActionResult AddActor(Actor actor) { ... }
```

---

### ⚠️ WARNING 5 — `ActorController.DeleteActor` POST removes an untracked entity

```csharp
// ActorController — DANGEROUS
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult DeleteActor(int id, Actor actor)
{
    _netflixContext.Actors.Remove(actor);  // ← actor is model-bound from form, NOT fetched from DB
    _netflixContext.SaveChanges();
    ...
}
```

If the form only posts `id`, the `actor` object will have all default/empty values. EF will either throw a `DbUpdateConcurrencyException` or silently do nothing depending on tracking state. `CategoryController` does this correctly — fetch first, then remove:

```csharp
// CategoryController — CORRECT
var category = await _context.Categories.FindAsync(id);
if (category == null) return NotFound();
_context.Categories.Remove(category);
```

**Fix `ActorController.DeleteActor` POST to match:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteActor(int id)
{
    var actor = await _netflixContext.Actors.FindAsync(id);
    if (actor == null) return NotFound();
    _netflixContext.Actors.Remove(actor);
    await _netflixContext.SaveChangesAsync();
    return RedirectToAction(nameof(GetAllActors));
}
```

---

### ⚠️ WARNING 6 — `ActorController` skips `ModelState.IsValid` on Add and Update

`CategoryController` and `ProfilesController` both check `ModelState.IsValid` before saving. `ActorController` wraps everything in a bare `try/catch` and never validates — invalid data goes straight to the DB and only fails if the DB rejects it.

```csharp
// ActorController — missing validation
[HttpPost]
public ActionResult AddActor(Actor actor)
{
    try
    {
        _netflixContext.Actors.Add(actor);   // ← no ModelState check
        _netflixContext.SaveChanges();
        ...
    }
}
```

**Fix:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddActor(Actor actor)
{
    if (!ModelState.IsValid)
        return View(actor);

    _netflixContext.Actors.Add(actor);
    await _netflixContext.SaveChangesAsync();
    return RedirectToAction(nameof(GetAllActors));
}
```

---

### ⚠️ WARNING 7 — `ProfilesController.DeleteProfile` GET and POST have no `[Authorize]`

```csharp
// ProfilesController — no auth on delete
public async Task<IActionResult> DeleteProfile(int id) { ... }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteProfileConfirmed(int id) { ... }
```

Any anonymous user can delete any profile. Every other write action in `ProfilesController` has `[Authorize]`. This is an oversight.

**Fix:**
```csharp
[Authorize]
public async Task<IActionResult> DeleteProfile(int id) { ... }

[HttpPost]
[Authorize]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteProfileConfirmed(int id) { ... }
```

---

### ⚠️ WARNING 8 — `ActorController` field is named `_netflixContext`, everything else uses `_context`

Minor naming inconsistency. `CategoryController` and `ProfilesController` both use `_context`. `ActorController` uses `_netflixContext`. Not a runtime issue but makes the codebase inconsistent.

---

### Summary

| # | Warning | Controller | Severity |
|---|---|---|---|
| W1 | `Update` POST returns `NotFound` instead of `BadRequest` on id mismatch | CategoryController | Low |
| W2 | No `[Authorize]` — entire controller is open | ActorController | 🔴 High |
| W3 | Synchronous DB calls block thread pool | ActorController | 🟡 Medium |
| W4 | Missing `[ValidateAntiForgeryToken]` on Add/Update POST | ActorController | 🔴 High |
| W5 | Delete removes untracked model-bound entity, not DB entity | ActorController | 🔴 High |
| W6 | No `ModelState.IsValid` check before saving | ActorController | 🟡 Medium |
| W7 | Delete GET and POST have no `[Authorize]` | ProfilesController | 🔴 High |
| W8 | Field named `_netflixContext` instead of `_context` | ActorController | ⚪ Low |

**CategoryController itself is solid** — W1 is the only fix needed in it (one line change). All other warnings are in `ActorController` and `ProfilesController`.
