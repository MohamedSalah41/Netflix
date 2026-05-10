# Model Report — Netflix Clone

## Inheritance Strategy: TPC (Table Per Concrete Type) ✅

`UseTpcMappingStrategy()` is correctly applied on `BaseItem`. This means:
- Abstract classes (`BaseItem`, `MediaItem`, `GeneralSeries`) → **no tables**
- Concrete classes (`Movie`, `Episode`, `Series`, `SeriesOfMovies`) → **each gets its own full table**
- A shared HiLo sequence (`BaseItemIdSequence`) handles unique IDs across all concrete tables

---

## Inheritance Tree

```
BaseItem (abstract)
├── MediaItem (abstract)
│   ├── Movie          → table: Movies
│   └── Episode        → table: Episodes
└── GeneralSeries (abstract)
    ├── Series         → table: Series
    └── SeriesOfMovies → table: SeriesOfMovies

Season        → table: Seasons       (standalone, not in TPC tree)
Actor         → table: Actors        (standalone)
Category      → table: Categories    (standalone)
Profile       → table: Profiles      (standalone)
WatchHistory  → table: WatchHistory  (standalone)
AppUser       → ASP.NET Identity tables
```

---

## Tables Summary

| Table | Key Source | Notable Columns |
|---|---|---|
| Movies | HiLo sequence | Name, Description, Poster, Rating, VideoUrl, DurationSeconds, SeriesOfMoviesId (FK, nullable) |
| Episodes | HiLo sequence | Name, Description, Poster, Rating, VideoUrl, DurationSeconds, Number, SeasonId (FK, nullable) |
| Series | HiLo sequence | Name, Description, Poster, Rating, TrailerUrl |
| SeriesOfMovies | HiLo sequence | Name, Description, Poster, Rating, TrailerUrl |
| Seasons | Identity | Name, Number, Poster, SeriesId (FK, nullable) |
| Actors | Identity | Name, Bio, Photo |
| Categories | Identity | Name |
| Profiles | Identity | Name, Image, IsKid, AppUserId (FK, nullable) |
| WatchHistory | Identity | ProfileId (FK), MediaItemId (no FK), Progress, LastWatchedUtc, IsFinished |

### Junction Tables (many-to-many)
| Table | Connects |
|---|---|
| MovieActors | Movies ↔ Actors |
| MovieCategories | Movies ↔ Categories |
| SeriesActors | Series ↔ Actors |
| SeriesCategories | Series ↔ Categories |
| SeriesOfMoviesActors | SeriesOfMovies ↔ Actors |
| SeriesOfMoviesCategories | SeriesOfMovies ↔ Categories |

---

## ⚠️ Warnings & Issues

### 1. `[MaxLength]` on model vs `HasMaxLength()` in Fluent API — mismatch
**Severity: Medium**

`BaseItem` declares `[MaxLength(50)]` on `Name` and `[MaxLength(500)]` on `Description` via data annotations, but `OnModelCreating` overrides them with `HasMaxLength(256)` and `HasMaxLength(2000)` respectively. The Fluent API wins at the DB level, but the data annotation still runs at the **validation layer** (model binding, form validation), so the UI will reject names longer than 50 chars while the DB allows up to 256. This is a silent inconsistency.

**Fix:** Remove the `[MaxLength]` annotations from `BaseItem` and rely solely on Fluent API, or align both values.

---

### 2. `WatchHistory.MediaItemId` has no foreign key constraint
**Severity: Medium — by design, but risky**

`MediaItemId` stores the PK of either a `Movie` or an `Episode`, but there is no FK constraint in the DB. This means:
- You can store a `MediaItemId` that points to a deleted or non-existent row — no referential integrity.
- The service layer must manually resolve which table to query.

**Recommendation:** Consider adding a `MediaItemType` discriminator column (`"Movie"` / `"Episode"`) to `WatchHistory` so the service layer knows which table to join without guessing.

---

### 3. `Season.Name` — `[MaxLength(20)]` vs Fluent `HasMaxLength(256)`
**Severity: Low**

Same annotation/fluent mismatch as issue #1. The `[MaxLength(20)]` annotation on `Season.Name` will block input over 20 chars at the UI level, but the DB column allows 256.

---

### 4. `Episode.Season` initialized as `null!`
**Severity: Low**

```csharp
public Season? Season { get; set; } = null!;
```
The property is nullable (`Season?`) but initialized with `null!` (null-forgiving). This is contradictory — `null!` suppresses the compiler warning but adds no real value here since the type is already nullable. It's harmless but misleading.

**Fix:** Change to simply `public Season? Season { get; set; }`.

---

### 5. `Profile.AppUserId` initialized as `string.Empty` but is nullable
**Severity: Low**

```csharp
public string? AppUserId { get; set; } = string.Empty;
```
The property is `string?` (nullable) but defaults to `""` instead of `null`. EF will store an empty string `""` instead of `NULL` if a profile is created without a user, which can break null checks in queries.

**Fix:** Change to `public string? AppUserId { get; set; }`.

---

### 6. `Actor` has no navigation back to Movies/Series
**Severity: Info**

`Actor` has no reverse navigation properties (e.g., `ICollection<Movie> Movies`). This is fine if you never need to query "all movies for a given actor", but worth noting if that use case comes up.

---

## What's Done Well ✅

- TPC is correctly set up with HiLo to avoid PK collisions across concrete tables
- Many-to-many relationships use explicit junction table names — clean and predictable
- `GeneralSeries` correctly avoids declaring `Actors`/`Categories` at the abstract level (TPC limitation handled properly)
- `OnDelete(Cascade)` is set appropriately on owned relationships (Season→Series, Episode→Season, Profile→AppUser, WatchHistory→Profile)
- `SeriesOfMovies → Movie` uses `SetNull` on delete — correct since a movie can exist independently
- ASP.NET Identity is properly extended via `AppUser : IdentityUser`
