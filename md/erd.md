# ERD — Netflix Clone Models

```
                        ┌─────────────────────────────┐
                        │         BaseItem            │
                        │   (abstract)                │
                        │─────────────────────────────│
                        │ PK  Id              int     │
                        │     Name            string  │
                        │     Description     string  │
                        │     Poster          string  │
                        │     Rating          decimal │
                        └──────────────┬──────────────┘
                                       │
                    ┌──────────────────┴──────────────────┐
                    │                                     │
       ┌────────────▼────────────┐           ┌───────────▼───────────┐
       │      GeneralSeries      │           │       MediaItem       │
       │      (abstract)         │           │       (abstract)      │
       │─────────────────────────│           │───────────────────────│
       │  TrailerUrl   string    │           │  VideoUrl      string │
       │  Actors       →Actor[]  │           │  DurationSeconds  int │
       │  Categories   →Cat[]    │           └─────────┬─────────────┘
       └────────────┬────────────┘                     │
                    │                        ┌──────────┴──────────┐
          ┌─────────┴──────────┐             │                     │
          │                    │    ┌────────▼────────┐   ┌────────▼────────┐
  ┌───────▼───────┐  ┌─────────▼──────────┐│      Movie      │   │     Episode     │
  │    Series     │  │   SeriesOfMovies   ││─────────────────│   │─────────────────│
  │───────────────│  │────────────────────││ SeriesOfMoviesId│   │ Number    int   │
  │ Seasons →     │  │ Movies → Movie[]   ││   int (FK)      │   │ SeasonId  int FK│
  │   Season[]    │  └────────────────────┘│ SeriesOfMovies  │   │ Season    →     │
  └───────┬───────┘           ▲            │                 │   │           Season│
          │                   │            │                 │   └────────┬────────┘
          │ 1                 │ 0..*       └────────┬────────┘            │
          ▼ *                 │                     │                     │
  ┌───────────────┐           └─────────────────────┘                     │
  │    Season     │                                                       │
  │  (no base)    │◄──────────────────────────────────────────────────────┘
  │───────────────│                  SeasonId FK
  │ PK Id    int  │
  │ FK SeriesId   │
  │    Series →   │
  │    Name string│
  │    Number int │
  │    Poster str?│
  │ Episodes →    │
  │   Episode[]   │
  └───────────────┘


  ┌─────────────────────┐        ┌─────────────────────┐
  │        Actor        │        │      Category       │
  │─────────────────────│        │─────────────────────│
  │ PK  Id     int      │        │ PK  Id    int       │
  │     Name   string   │        │     Name  string    │
  │     Bio    string?  │        └─────────────────────┘
  │     Photo  string?  │
  └─────────────────────┘
```

## Many-to-Many Relationships
(junction tables configured by Member 2 in DbContext)

| Owner              | Target     | Junction Table              |
|--------------------|------------|-----------------------------|
| GeneralSeries      | Actor      | GeneralSeriesActors         |
| GeneralSeries      | Category   | GeneralSeriesCategories     |
| Movie (standalone) | Actor      | BaseItemActors              |
| Movie (standalone) | Category   | BaseItemCategories          |

## Inheritance Hierarchy

```
BaseItem  (abstract)
├── GeneralSeries  (abstract)
│   ├── Series           ← has Seasons
│   └── SeriesOfMovies   ← has Movies
└── MediaItem  (abstract)
    ├── Movie            ← optional FK to SeriesOfMovies
    └── Episode          ← FK to Season

Season  (no base class — structural container)
```

## TPH Note
All `BaseItem` subtypes map to a **single `BaseItems` table** with a `Discriminator` column.  
`Season` is outside the TPH tree entirely.
