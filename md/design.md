# Watchit — ERD & Data Model Design

## Inheritance Strategy: TPH (Table Per Hierarchy)

EF Core stores all `BaseItem` subtypes in **one table** (`BaseItems`) with a `Discriminator` column.  
No joins needed for browsing — simpler queries, better performance at this scale.

---

## Inheritance Trees

```
BaseItem
├── GeneralSeries
│   ├── Series
│   └── SeriesOfMovies
└── MediaItem
    ├── Movie
    └── Episode
```

> `Season` is **not** a `BaseItem` — it's a structural container, not a browsable/playable item.

---

## ERD Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        BaseItems (TPH)                          │
│─────────────────────────────────────────────────────────────────│
│ Id               int          PK                                │
│ Name             string       required                          │
│ Description      string                                         │
│ Poster           string       URL                               │
│ Rating           decimal(3,1) aggregate, updated on review      │
│ Discriminator    string       EF-managed                        │
│ ── GeneralSeries only ──────────────────────────────────────── │
│ TrailerUrl       string                                         │
│ ── MediaItem only ──────────────────────────────────────────── │
│ VideoUrl         string                                         │
│ DurationSeconds  int                                            │
│ ── Episode only ────────────────────────────────────────────── │
│ Number           int          episode number within season      │
│ SeasonId         int          FK → Seasons                      │
│ ── Movie only ──────────────────────────────────────────────── │
│ SeriesOfMoviesId int          FK → BaseItems (nullable)         │
└─────────────────────────────────────────────────────────────────┘
         │                          │
         │ 1                        │ 1
         ▼ *                        ▼ *
┌──────────────────┐     ┌──────────────────────────┐
│     Seasons      │     │  GeneralSeriesActors      │
│──────────────────│     │──────────────────────────│
│ Id        int PK │     │ GeneralSeriesId  int  FK  │
│ SeriesId  int FK │     │ ActorId          int  FK  │
│ Name      string │     └──────────────────────────┘
│ Number    int    │
│ Poster    string │     ┌──────────────────────────┐
└──────────────────┘     │ GeneralSeriesCategories  │
         │               │──────────────────────────│
         │ 1             │ GeneralSeriesId  int  FK  │
         ▼ *             │ CategoryId       int  FK  │
┌──────────────────┐     └──────────────────────────┘
│  (Episodes are   │
│  in BaseItems    │     ┌──────────────────────────┐
│  via TPH)        │     │    BaseItemActors         │
└──────────────────┘     │  (standalone Movies)     │
                         │──────────────────────────│
                         │ BaseItemId  int  FK       │
                         │ ActorId     int  FK       │
                         └──────────────────────────┘

                         ┌──────────────────────────┐
                         │   BaseItemCategories      │
                         │  (standalone Movies)      │
                         │──────────────────────────│
                         │ BaseItemId   int  FK      │
                         │ CategoryId   int  FK      │
                         └──────────────────────────┘

┌─────────────────────┐       ┌─────────────────────┐
│       Actors        │       │      Categories     │
│─────────────────────│       │─────────────────────│
│ Id     int    PK    │       │ Id    int    PK     │
│ Name   string       │       │ Name  string        │
│ Bio    string       │       └─────────────────────┘
│ Photo  string  URL  │
└─────────────────────┘

┌──────────────────────────────────────────┐
│         AspNetUsers (AppUser)            │
│──────────────────────────────────────────│
│ Id      string  PK  GUID (from Identity) │
│ Email   string                           │
│ (+ all default Identity columns)         │
└──────────────────────────────────────────┘
                  │ 1
                  ▼ *
┌──────────────────────────────────────────┐
│                Profiles                  │
│──────────────────────────────────────────│
│ Id          int     PK                   │
│ AppUserId   string  FK → AspNetUsers     │
│ Name        string                       │
│ Image       string  URL                  │
│ IsKid       bool    enables kid-safe mode│
└──────────────────────────────────────────┘
                  │ 1
                  ▼ *
┌──────────────────────────────────────────┐
│              WatchHistory                │
│──────────────────────────────────────────│
│ Id               int       PK            │
│ ProfileId        int       FK → Profiles │
│ MediaItemId      int       FK → BaseItems│
│ Progress         TimeSpan                │
│ LastWatchedUtc   datetime                │
│ IsFinished       bool                    │
└──────────────────────────────────────────┘
```

---

## Tables Reference

### BaseItems (TPH root)

| Column | Type | Notes |
|---|---|---|
| Id | int | PK, auto-increment |
| Name | string | required |
| Description | string | |
| Poster | string | URL |
| Rating | decimal(3,1) | aggregate, updated on review |
| Discriminator | string | EF-managed |
| TrailerUrl | string | GeneralSeries subtypes only |
| VideoUrl | string | MediaItem subtypes only |
| DurationSeconds | int | MediaItem subtypes only |
| Number | int | Episode only |
| SeasonId | int | FK → Seasons (Episode only) |
| SeriesOfMoviesId | int | FK → BaseItems, nullable (Movie only) |

### Actors

| Column | Type |
|---|---|
| Id | int PK |
| Name | string |
| Bio | string |
| Photo | string (URL) |

### Categories

| Column | Type |
|---|---|
| Id | int PK |
| Name | string |

### Seasons

| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| SeriesId | int | FK → BaseItems (Discriminator = Series) |
| Name | string | e.g. "Season 1" |
| Number | int | |
| Poster | string | optional |

### AspNetUsers (AppUser extends IdentityUser)

| Column | Type | Notes |
|---|---|---|
| Id | string | PK, GUID from Identity |
| Email | string | |
| *(all default Identity columns)* | | |

### Profiles

| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| AppUserId | string | FK → AspNetUsers |
| Name | string | |
| Image | string | URL |
| IsKid | bool | enables kid-safe mode |

### WatchHistory

| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| ProfileId | int | FK → Profiles |
| MediaItemId | int | FK → BaseItems (Movie or Episode only) |
| Progress | TimeSpan | stored as `time` in SQL |
| LastWatchedUtc | datetime | |
| IsFinished | bool | |

### Junction Tables

| Table | Columns |
|---|---|
| GeneralSeriesActors | GeneralSeriesId (FK), ActorId (FK) |
| GeneralSeriesCategories | GeneralSeriesId (FK), CategoryId (FK) |
| BaseItemActors | BaseItemId (FK), ActorId (FK) |
| BaseItemCategories | BaseItemId (FK), CategoryId (FK) |

---

## Relationships Summary

| From | To | Type | Notes |
|---|---|---|---|
| AppUser | Profile | 1 → many | user can have N profiles |
| Profile | WatchHistory | 1 → many | per-profile history |
| MediaItem | WatchHistory | 1 → many | Movie or Episode only |
| Series | Season | 1 → many | |
| Season | Episode | 1 → many | via SeasonId on BaseItems |
| SeriesOfMovies | Movie | 1 → many | nullable FK, movie can be standalone |
| GeneralSeries | Actor | many → many | via GeneralSeriesActors |
| GeneralSeries | Category | many → many | via GeneralSeriesCategories |
| Movie (standalone) | Actor | many → many | via BaseItemActors |
| Movie (standalone) | Category | many → many | via BaseItemCategories |

---

## C# Entity Classes

```csharp
// ── Base ────────────────────────────────────────────────────────

public abstract class BaseItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public decimal Rating { get; set; }  // e.g. 8.4
}

// ── Series hierarchy ────────────────────────────────────────────

public abstract class GeneralSeries : BaseItem
{
    public string TrailerUrl { get; set; } = string.Empty;
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}

public class Series : GeneralSeries
{
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}

public class SeriesOfMovies : GeneralSeries
{
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}

// ── Playable items ──────────────────────────────────────────────

public abstract class MediaItem : BaseItem
{
    public string VideoUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}

public class Movie : MediaItem
{
    // nullable — movie can be standalone or part of a franchise
    public int? SeriesOfMoviesId { get; set; }
    public SeriesOfMovies? SeriesOfMovies { get; set; }

    // actors/categories for standalone movies
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}

public class Episode : MediaItem
{
    public int Number { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}

// ── Structural (not BaseItem) ────────────────────────────────────

public class Season
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;
    public string Name { get; set; } = string.Empty;  // "Season 1"
    public int Number { get; set; }
    public string? Poster { get; set; }
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}

// ── Supporting models ────────────────────────────────────────────

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Photo { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ── Identity & Profiles ──────────────────────────────────────────

public class AppUser : IdentityUser
{
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}

public class Profile
{
    public int Id { get; set; }
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsKid { get; set; }
    public ICollection<WatchHistory> WatchHistory { get; set; } = new List<WatchHistory>();
}

// ── Watch History ────────────────────────────────────────────────

public class WatchHistory
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public int MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; } = null!;  // Movie or Episode only
    public TimeSpan Progress { get; set; }
    public DateTime LastWatchedUtc { get; set; }
    public bool IsFinished { get; set; }
}
```

---

## Key Design Decisions

1. **Standalone Movie** — `SeriesOfMoviesId` on `Movie` is nullable. A movie can exist by itself or belong to a franchise. Actors/Categories for standalone movies go through `BaseItemActors` / `BaseItemCategories`.

2. **TPH inheritance** — all `BaseItem` subtypes live in one table. Nullable columns are fine since EF only populates what's relevant per discriminator. Simpler queries, no joins for browsing.

3. **Rating is decimal on BaseItem** — updated as an aggregate when users submit ratings (Sprint 3). Per-user star rating lives in a future `Rating` table in Sprint 3.

4. **Season is NOT a BaseItem** — seasons are structural containers, not browsable/playable items. They sit outside the TPH tree entirely.

5. **WatchHistory.MediaItemId** points only to `Movie` and `Episode`. Enforce this in the service layer — TPH can't enforce discriminator-scoped FK constraints at the DB level.

---

## Handoff Checklist (before Member 2 runs migration)

- [ ] Agree on nullability for optional fields: `Bio`, `Photo`, `SeriesOfMoviesId`
- [ ] Confirm `Season` stays outside the `BaseItem` tree
- [ ] Confirm junction table names match exactly — casing matters for EF conventions
- [ ] Seed: at least 2 Categories, 1 Actor, 1 AppUser with Admin role
- [ ] No renaming after Member 3 starts — `AppUser`, `Profile`, `WatchHistory` are final names

---

## Notes & Flags

> **Flag 1 — Two actor junction tables**  
> `GeneralSeries` uses `GeneralSeriesActors` and standalone `Movie` uses `BaseItemActors`. This means actor queries for a movie inside a `SeriesOfMovies` need to decide which table to hit. Recommendation: always use `BaseItemActors` for `Movie` regardless of whether it belongs to a franchise, and drop `GeneralSeriesActors` for `SeriesOfMovies`. Discuss with Member 2 before migration.

> **Flag 2 — SeriesId on Season**  
> `Season.SeriesId` is an FK into `BaseItems` filtered by `Discriminator = 'Series'`. EF can't enforce this at the DB level with TPH. Add a check in the service layer or use a fluent API comment for clarity.

> **Flag 3 — DurationSeconds vs TimeSpan**  
> Storing duration as `int DurationSeconds` on `MediaItem` is simpler for SQL. Convert to `TimeSpan` in the view layer when displaying "1h 42m". Avoids SQL `time` type limitations (max 24h).

> **Flag 4 — Rating precision**  
> `decimal(3,1)` supports values like `9.9` but not `10.0`. Change to `decimal(4,1)` if you want to allow a perfect 10.
