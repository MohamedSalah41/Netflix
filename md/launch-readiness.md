# Launch Readiness Report

## What's Missing & What Needs to Be Fixed Before Launch

---

## 1. Static / Hardcoded Data That Must Come From the Database

These are the most critical issues. Real data exists in the DB but the UI ignores it and shows fake values instead.

---

### 1.1 Navbar — Profile List (CRITICAL)

**File:** `Views/Shared/_Layout.cshtml` — the `@{ }` block at the top

**Problem:** The profile switcher in the navbar dropdown is built from a hardcoded C# array:

```csharp
var profiles = new[]
{
    new { Id = 1, Name = "Ahmed",   Avatar = "AH", Color = "#e50914", IsKid = false, IsActive = true  },
    new { Id = 2, Name = "Sara",    Avatar = "SA", Color = "#3b82f6", IsKid = false, IsActive = false },
    new { Id = 3, Name = "Kids",    Avatar = "★",  Color = "#f59e0b", IsKid = true,  IsActive = false }
};
```

**What it should do:** Query `_context.Profiles.Where(p => p.AppUserId == currentUserId)` and pass the result to the layout via a base controller or `ViewBag`.

**Where the real data lives:** `Profiles` table → `Profile` model → linked to `AppUser` via `AppUserId`.

---

### 1.2 Navbar — Browse Categories Dropdown (CRITICAL)

**File:** `Views/Shared/_Layout.cshtml` — the `@{ }` block at the top

**Problem:** The "Browse" mega-dropdown is populated from a hardcoded string array:

```csharp
var navCategories = new[]
{
    "Action", "Drama", "Comedy", "Thriller", "Horror",
    "Sci-Fi", "Romance", "Documentary", "Animation", "Crime"
};
```

**What it should do:** Query `_context.Categories.OrderBy(c => c.Name).ToList()` and inject via `ViewBag` from a base controller.

**Where the real data lives:** `Categories` table → `Category` model.

---

### 1.3 Navbar — Notifications (CRITICAL)

**File:** `Views/Shared/_Layout.cshtml` — the `@{ }` block at the top

**Problem:** Notifications are completely fake:

```csharp
var notifications = new[]
{
    new { Icon = "🎬", Text = "New episodes of Breaking Bad added",  Time = "2h ago",  IsRead = false },
    new { Icon = "⭐", Text = "Your rating on Inception was saved",  Time = "5h ago",  IsRead = false },
    ...
};
```

**What it should do:** Either build a `Notification` model and table, or remove the bell icon entirely until the feature is implemented.

**Where the real data lives:** Nowhere yet — the `Notification` entity and table do not exist in the schema.

---

### 1.4 Navbar — Subscription Plan Badge

**File:** `Views/Shared/_Layout.cshtml`

**Problem:** The subscription tier shown in the profile dropdown is hardcoded:

```csharp
var subscriptionPlan = "Premium";
```

**What it should do:** Either add a `SubscriptionPlan` field to `AppUser` and read it, or remove the badge until subscriptions are implemented.

**Where the real data lives:** Nowhere — `AppUser` has no subscription field.

---

### 1.5 Home Page — "For You" Row Is Random, Not Personalized

**File:** `Controllers/HomeController.cs`

**Problem:** "For You" is just a random shuffle of all content:

```csharp
var forYou = allRows.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
```

It is not personalized at all. Same for "New Releases" which is also random:

```csharp
var newReleases = allRows.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
```

**What it should do:**
- "For You" → filter by categories the user has watched before (via `WatchHistory`)
- "New Releases" → order by a `ReleaseDate` field (which does not exist yet on `BaseItem`)

---

### 1.6 Home Page — Hero Year Is Always `DateTime.Now.Year`

**File:** `Controllers/HomeController.cs`

**Problem:**

```csharp
int heroYear = DateTime.Now.Year;
```

The year shown on the hero banner is always the current year regardless of when the content was actually released.

**What it should do:** Read from a `ReleaseYear` or `ReleaseDate` field on the content item.

**Where the real data lives:** Nowhere — `BaseItem` has no release date field.

---

### 1.7 Season Edit Form — SeriesId Is a Raw Number Input

**File:** `Views/Season/UpdateSeason.cshtml`

**Problem:** The "Edit Season" form shows a plain number input for `SeriesId`:

```html
<input asp-for="SeriesId" class="form-input" type="number" />
```

The Add Season form correctly uses a `<select>` dropdown populated from `ViewBag.SeriesList`, but the Update form does not.

**What it should do:** Use the same `<select asp-items>` pattern as `AddSeason.cshtml`, and `SeasonController.UpdateSeason(int id)` already passes `ViewBag.SeriesList` — the view just doesn't use it.

---

### 1.8 Episode List — Shows Raw Season ID Instead of Season Name

**File:** `Views/Episode/GetAllEpisodes.cshtml`

**Problem:**

```html
<div class="episode-meta">
    Season ID: @item.SeasonId
</div>
```

Displays the raw foreign key integer, not the season name.

**What it should do:** Include the `Season` navigation property in the query (`context.Episodes.Include(e => e.Season).ToList()`) and display `item.Season?.Name`.

---

### 1.9 Season List — Shows Raw Series ID Instead of Series Name

**File:** `Views/Season/GetAllSeasons.cshtml`

**Problem:**

```html
<th>Series ID</th>
...
<td>@(item.SeriesId.HasValue ? item.SeriesId.ToString() : "—")</td>
```

Displays the raw FK integer.

**What it should do:** Include `Series` in the query and display `item.Series?.Name`.

---

## 2. Missing Features (Not Implemented At All)

---

### 2.1 No Role Seeding / Admin Account Creation

**Problem:** `[Authorize(Roles = "Admin")]` is used on `UsersController` and `ProfilesController.GetAllProfiles`, but there is no code anywhere that creates the "Admin" role or assigns it to a user. The app will start but no one can access admin pages.

**What's needed:** A database seeder in `Program.cs` that calls `RoleManager.CreateAsync(new IdentityRole("Admin"))` and optionally seeds a default admin user.

---

### 2.2 No "My List" / Watchlist Feature

**Problem:** The Home page hero has an "Add To My List" button that links to `href="#"` — it does nothing.

```html
<a href="#" class="hero-btn hero-btn--list">
    + Add To My List
</a>
```

**What's needed:** A `Watchlist` or `MyList` table, a controller action to add/remove items, and wiring up the button.

---

### 2.3 No Video Player / Watch Page

**Problem:** The "Watch" button on the hero links to the Details page, not a dedicated watch/player page. The Details page for movies embeds the `VideoUrl` directly in an `<iframe>`, which only works for YouTube/external URLs — it will not work for local video files.

**What's needed:** A dedicated `/Watch/{id}` route that renders a proper `<video>` player and updates `WatchHistory` on play/pause/end.

---

### 2.4 WatchHistory Is Never Written

**Problem:** The `WatchHistory` table and model exist, and `HomeController` reads from it for "Continue Watching". But there is no controller action anywhere that writes to `WatchHistory`. The "Continue Watching" row will always be empty.

**What's needed:** A `WatchController` or API endpoint that accepts `{ profileId, mediaItemId, progress }` and upserts a `WatchHistory` record.

---

### 2.5 Movie Form Missing Categories and Actors

**Problem:** `MovieController.AddMovie` and `Edit` do not pass `ViewBag.Categories` or `ViewBag.Actors` to the view, and the Add/Edit Movie views have no multi-select for categories or actors. The `Movie` model has `ICollection<Actor> Actors` and `ICollection<Category> Categories` but they are never set when creating or editing a movie.

**What's needed:** Same pattern as `SeriesController.AddSeries` — load actors and categories into `ViewBag`, add multi-selects to the view, and handle the posted `actorIds`/`categoryIds` lists in the POST action.

---

### 2.6 No Search Functionality

**Problem:** The navbar has a search input that expands on click, but it is purely cosmetic — there is no form action, no controller, and no search results page.

**What's needed:** A `SearchController.Index(string q)` action that queries `Movies` and `Series` by name/description and returns a results view.

---

### 2.7 No Pagination on Any List Page

**Problem:** `GetAllMovies`, `GetAllSeries`, `GetAllActors`, `GetAllEpisodes`, `GetAllSeasons` all load the entire table with no pagination. With real data this will be slow and unusable.

**What's needed:** Server-side pagination (page/pageSize query params) or at minimum a `Take(50)` limit.

---

### 2.8 No `ReleaseDate` / `ReleaseYear` on Content

**Problem:** `BaseItem` has no date field. The hero year defaults to `DateTime.Now.Year`. "New Releases" cannot be sorted by release date.

**What's needed:** Add `int ReleaseYear` or `DateTime ReleaseDate` to `BaseItem`, create a new migration, and use it in the Home controller and views.

---

### 2.9 `SeriesOfMovies` Has No Controller or Views

**Problem:** The `SeriesOfMovies` model and DB table exist (it's a franchise/collection grouping for movies), but there is no `SeriesOfMoviesController` and no views for it. The `Movie` form also has no way to assign a movie to a `SeriesOfMovies`.

**What's needed:** Either build the CRUD for it or remove the concept from the schema if it won't be used at launch.

---

### 2.10 `DeleteSeries` POST Uses Wrong Approach

**Problem:** `SeriesController.DeleteSeries(int id, Series series)` removes the `series` object passed from the form, but the form only posts the `Id` — the rest of the object is empty/default. This will likely cause an EF tracking error or delete the wrong record.

**What's needed:** Load the entity from the DB by `id` before removing it, same pattern used in `CategoryController.DeleteConfirmed`.

---

### 2.11 No Authorization on Content Management Controllers

**Problem:** `MovieController`, `SeriesController`, `ActorController`, `EpisodeController`, `SeasonController` have no `[Authorize]` attributes. Any anonymous visitor can add, edit, or delete movies and series.

**What's needed:** Add `[Authorize(Roles = "Admin")]` to the POST actions (Add, Edit, Delete) on all content management controllers.

---

### 2.12 Profile Views Are Unstyled

**Problem:** `AddProfile.cshtml`, `UpdateProfile.cshtml`, `GetAllProfiles.cshtml`, and `Views/Users/Index.cshtml` use plain Bootstrap classes (`form-group`, `control-label`, `table`) that are not part of the custom design system. They will look completely out of place compared to the rest of the app.

**What's needed:** Restyle these views to use the same `form-card`, `form-field`, `form-label`, `form-input`, `pg-table` classes used everywhere else.

---

## 3. Summary Table

| # | Area | Issue | Severity |
|---|------|-------|----------|
| 1.1 | Navbar | Profile list is hardcoded, not from DB | 🔴 Critical |
| 1.2 | Navbar | Browse categories are hardcoded, not from DB | 🔴 Critical |
| 1.3 | Navbar | Notifications are fake, no table exists | 🟡 Medium |
| 1.4 | Navbar | Subscription plan is hardcoded | 🟡 Medium |
| 1.5 | Home | "For You" and "New Releases" are random, not real | 🟡 Medium |
| 1.6 | Home | Hero year is always current year | 🟡 Medium |
| 1.7 | Season Edit | SeriesId is a number input, not a dropdown | 🟠 High |
| 1.8 | Episode List | Shows raw SeasonId FK instead of season name | 🟠 High |
| 1.9 | Season List | Shows raw SeriesId FK instead of series name | 🟠 High |
| 2.1 | Auth | No role seeding — Admin role never created | 🔴 Critical |
| 2.2 | Feature | "Add To My List" button does nothing | 🟡 Medium |
| 2.3 | Feature | No dedicated watch/player page | 🟠 High |
| 2.4 | Feature | WatchHistory is never written | 🟠 High |
| 2.5 | Movie | Add/Edit Movie has no actors or categories | 🔴 Critical |
| 2.6 | Feature | Search bar is cosmetic only | 🟡 Medium |
| 2.7 | Performance | No pagination on any list page | 🟠 High |
| 2.8 | Schema | No ReleaseDate/ReleaseYear on content | 🟡 Medium |
| 2.9 | Feature | SeriesOfMovies has no controller or views | 🟡 Medium |
| 2.10 | Bug | DeleteSeries POST deletes empty object | 🔴 Critical |
| 2.11 | Security | Content controllers have no authorization | 🔴 Critical |
| 2.12 | UI | Profile/User views are unstyled | 🟠 High |

---

## 4. Recommended Fix Order

1. **Role seeding** (2.1) — nothing admin-protected works without this
2. **Authorization on content controllers** (2.11) — security hole
3. **DeleteSeries bug** (2.10) — data corruption risk
4. **Navbar profiles from DB** (1.1) — every logged-in user sees wrong profiles
5. **Navbar categories from DB** (1.2) — Browse dropdown is fake
6. **Movie actors/categories** (2.5) — movies are incomplete without this
7. **Season edit dropdown** (1.7) — UX bug, easy fix
8. **Episode/Season list FK display** (1.8, 1.9) — confusing for admins
9. **WatchHistory writes + Watch page** (2.3, 2.4) — core streaming feature
10. **Profile/User view styling** (2.12) — polish before launch
11. **Search** (2.6), **Pagination** (2.7), **My List** (2.2) — nice-to-have for launch
