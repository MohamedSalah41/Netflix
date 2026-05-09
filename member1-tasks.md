# Member 1 — Implementation Tasks
## DB Models: Core Hierarchy

Work through these tasks **in order**. Do not move to the next task until the current one is reviewed.

---

## Task 1 — `BaseItem.cs`

**File:** `Models/BaseItem.cs`

Create the abstract base class that every other model inherits from.

**Properties to include:**
- `Id` → `int`, primary key
- `Name` → `string`, required
- `Description` → `string`
- `Poster` → `string` (URL)
- `Rating` → `decimal`, precision (4,1) — supports up to 10.0

**Notes:**
> - Mark the class `abstract` — it should never be instantiated directly
> - Use `decimal` not `double` for Rating — EF maps it to SQL `decimal` which avoids floating point drift
> - Use `decimal(4,1)` not `(3,1)` — allows a perfect 10.0 rating
> - Initialize `string` properties to `string.Empty` to avoid nullable warnings
> - Do NOT add `[Table]` or any EF attributes here — TPH configuration goes in DbContext later (Member 2's job)

**Done when:** Class is abstract, has all 5 properties, compiles with no warnings.

---

## Task 2 — `Actor.cs`

**File:** `Models/Actor.cs`

Simple standalone model, no inheritance.

**Properties to include:**
- `Id` → `int`, primary key
- `Name` → `string`, required
- `Bio` → `string?`, nullable
- `Photo` → `string?`, nullable (URL)

**Notes:**
> - `Bio` and `Photo` are nullable — not every actor will have them at seed time
> - No navigation properties here yet — EF will wire the many-to-many from the `GeneralSeries` side
> - Keep it simple, no base class

**Done when:** Class has all 4 properties, nullable annotations are correct, compiles clean.

---

## Task 3 — `Category.cs`

**File:** `Models/Category.cs`

Simplest model in the project.

**Properties to include:**
- `Id` → `int`, primary key
- `Name` → `string`, required

**Notes:**
> - No base class, no navigation properties on this side
> - Examples of values: "Action", "Drama", "Comedy", "Thriller"
> - Member 2 will seed at least 2 categories — make sure `Name` is required (not nullable)

**Done when:** Class has 2 properties, compiles clean.

---

## Task 4 — `GeneralSeries.cs`

**File:** `Models/GeneralSeries.cs`

Abstract class, inherits `BaseItem`, shared base for both series types.

**Properties to include:**
- `TrailerUrl` → `string`
- `Actors` → `ICollection<Actor>` (many-to-many)
- `Categories` → `ICollection<Category>` (many-to-many)

**Notes:**
> - Mark the class `abstract` — only `Series` and `SeriesOfMovies` get instantiated
> - Initialize both collections to `new List<Actor>()` and `new List<Category>()` — never leave collections null
> - `TrailerUrl` can be empty string default — not every item will have a trailer at seed time
> - The junction tables (`GeneralSeriesActors`, `GeneralSeriesCategories`) are configured by Member 2 in DbContext — you just define the navigation properties here

**Done when:** Class is abstract, inherits `BaseItem`, has `TrailerUrl` + 2 collections, compiles clean.

---

## Task 5 — `Series.cs`

**File:** `Models/Series.cs`

Concrete class, inherits `GeneralSeries`. Represents a show with seasons.

**Properties to include:**
- `Seasons` → `ICollection<Season>` (one-to-many)

**Notes:**
> - This is the first **concrete** class in the series hierarchy — not abstract
> - `Season` doesn't exist yet (Task 8) — you can forward-declare or just leave a `// TODO: add Season` comment and come back after Task 8
> - Do NOT add `SeriesId` here — that FK lives on `Season`, not here
> - EF will figure out the one-to-many from the navigation property alone

**Done when:** Class inherits `GeneralSeries`, has `Seasons` collection, compiles clean (after Task 8 is done).

---

## Task 6 — `SeriesOfMovies.cs`

**File:** `Models/SeriesOfMovies.cs`

Concrete class, inherits `GeneralSeries`. Represents a franchise or movie collection.

**Properties to include:**
- `Movies` → `ICollection<Movie>` (one-to-many)

**Notes:**
> - `Movie` doesn't exist yet (Task 7) — same approach as Task 5, come back to wire it after Task 7
> - A `Movie` can belong to this collection OR be standalone — the nullable FK (`SeriesOfMoviesId`) lives on `Movie`, not here
> - Do NOT add any FK property here

**Done when:** Class inherits `GeneralSeries`, has `Movies` collection, compiles clean (after Task 7 is done).

---

## Task 7 — `MediaItem.cs` + `Movie.cs` + `Episode.cs`

Do these three together — they're tightly coupled.

### `MediaItem.cs`
**File:** `Models/MediaItem.cs`

**Properties to include:**
- `VideoUrl` → `string`
- `DurationSeconds` → `int`

**Notes:**
> - Mark `abstract` — never instantiated directly
> - Store duration as `int` seconds, not `TimeSpan` — simpler SQL column, convert to "1h 42m" in the view layer
> - Inherits `BaseItem`

---

### `Movie.cs`
**File:** `Models/Movie.cs`

**Properties to include:**
- `SeriesOfMoviesId` → `int?`, nullable FK
- `SeriesOfMovies` → `SeriesOfMovies?`, nullable navigation property
- `Actors` → `ICollection<Actor>` (many-to-many, for standalone movies)
- `Categories` → `ICollection<Category>` (many-to-many, for standalone movies)

**Notes:**
> - `SeriesOfMoviesId` is nullable — a movie can exist without belonging to a franchise
> - Initialize `Actors` and `Categories` collections — these map to `BaseItemActors` / `BaseItemCategories` junction tables
> - Inherits `MediaItem`

---

### `Episode.cs`
**File:** `Models/Episode.cs`

**Properties to include:**
- `Number` → `int` (episode number within its season)
- `SeasonId` → `int`, FK
- `Season` → `Season`, navigation property

**Notes:**
> - `Season` doesn't exist yet — wire it after Task 8
> - `Number` is the episode number within the season (1, 2, 3...), not a global number
> - Inherits `MediaItem`

**Done when:** All three files exist, inherit correctly, compile clean (after Task 8 for `Episode`).

---

## Task 8 — `Season.cs`

**File:** `Models/Season.cs`

Structural container — does NOT inherit `BaseItem`.

**Properties to include:**
- `Id` → `int`, primary key
- `SeriesId` → `int`, FK → `Series`
- `Series` → `Series`, navigation property
- `Name` → `string` (e.g. "Season 1")
- `Number` → `int`
- `Poster` → `string?`, nullable
- `Episodes` → `ICollection<Episode>` (one-to-many)

**Notes:**
> - No base class — `Season` is a structural container, not a browsable item
> - `Poster` is nullable — not every season needs its own poster image
> - After creating this file, go back and complete the `Season` reference in `Series.cs` (Task 5) and `Episode.cs` (Task 7)
> - `SeriesId` FK points into `BaseItems` table at the DB level (TPH), but in C# it just points to `Series` — EF handles the rest

**Done when:** Class has all 7 properties, no base class, `Episodes` collection initialized, compiles clean.

---

## Final Checklist Before Handing Off to Member 2

- [ ] All 9 files exist in the `Models/` folder
- [ ] No class has EF attributes (`[Table]`, `[Column]`, etc.) — that's Member 2's job in DbContext
- [ ] All `string` properties are either initialized to `string.Empty` or marked nullable `string?`
- [ ] All collections are initialized (`new List<T>()`) — never null
- [ ] `BaseItem`, `GeneralSeries`, `MediaItem` are all `abstract`
- [ ] `Season` has NO base class
- [ ] `Movie.SeriesOfMoviesId` is `int?` (nullable)
- [ ] Project builds with `dotnet build` — zero errors, zero warnings
- [ ] Member 2 has reviewed and approved naming before migration runs
