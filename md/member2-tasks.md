# Member 2 — Implementation Tasks
## DbContext, Identity, WatchHistory, Migration & Seed

Work through these tasks **in order**. Do not move to the next task until the current one is reviewed.

> **Prerequisite:** Member 1's models must be complete and building clean before you start.  
> Confirm `dotnet build` passes with zero errors before Task 1.

---

## Task 1 — Install Required NuGet Packages

**No file — run these commands in the project root.**

Install the following packages:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

**Notes:**
> - Use the same major version for all three EF packages — mixing versions causes build errors
> - `Microsoft.EntityFrameworkCore.Tools` is needed for `dotnet ef migrations add` and `dotnet ef database update`
> - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` brings in `IdentityDbContext<TUser>` which you'll extend in Task 2
> - After installing, confirm `dotnet build` still passes before moving on

**Done when:** All three packages appear in `Netflix-clone.csproj`, project builds clean.

---

## Task 2 — `AppUser.cs` + `AppRole.cs`

Do these two together — Identity needs both before DbContext can be configured.

### `AppUser.cs`
**File:** `Models/Identity/AppUser.cs`

Extends `IdentityUser` — the ASP.NET Core Identity base for users.

**Properties to include:**
- `Profiles` → `ICollection<Profile>` (one-to-many navigation)

**Notes:**
> - Inherit from `IdentityUser` — do NOT redefine `Id`, `Email`, `UserName`, etc. Identity provides those
> - `IdentityUser.Id` is a `string` (GUID) — this is intentional, do not change it to `int`
> - Initialize `Profiles` to `new List<Profile>()` — never leave collections null
> - `Profile` doesn't exist yet (Task 3) — add a `// TODO` comment and come back after Task 3
> - Namespace: `Netflix_clone.Models.Identity`

---

### `AppRole.cs`
**File:** `Models/Identity/AppRole.cs`

Extends `IdentityRole` — needed to register a typed role in `IdentityDbContext`.

**Properties to include:**
- No extra properties needed — just inherit `IdentityRole`

**Notes:**
> - Even though it's empty now, having a custom `AppRole` class lets you add role-specific properties later without a migration headache
> - Namespace: `Netflix_clone.Models.Identity`

**Done when:** Both files exist, `AppUser` inherits `IdentityUser`, `AppRole` inherits `IdentityRole`, project builds clean.

---

## Task 3 — `Profile.cs`

**File:** `Models/Profiles/Profile.cs`

Represents a viewer profile under an account (like Netflix's "Who's watching?" screen).

**Properties to include:**
- `Id` → `int`, primary key
- `AppUserId` → `string`, FK → `AppUser` (string because Identity uses GUID)
- `AppUser` → `AppUser`, navigation property
- `Name` → `string`, required
- `Image` → `string?`, nullable (URL)
- `IsKid` → `bool` (enables kid-safe mode)
- `WatchHistory` → `ICollection<WatchHistory>` (one-to-many navigation)

**Notes:**
> - `AppUserId` is `string` not `int` — must match `IdentityUser.Id` type exactly or EF will throw
> - Initialize `WatchHistory` to `new List<WatchHistory>()` — never null
> - `WatchHistory` doesn't exist yet (Task 4) — add a `// TODO` comment and come back after Task 4
> - After creating this file, go back to `AppUser.cs` (Task 2) and remove the `// TODO` comment — the `Profiles` collection is now wired
> - Namespace: `Netflix_clone.Models.Profiles`

**Done when:** Class has all 7 properties, `AppUserId` is `string`, collections initialized, compiles clean.

---

## Task 4 — `WatchHistory.cs`

**File:** `Models/History/WatchHistory.cs`

Tracks per-profile playback progress for any `Movie` or `Episode`.

**Properties to include:**
- `Id` → `int`, primary key
- `ProfileId` → `int`, FK → `Profile`
- `Profile` → `Profile`, navigation property
- `MediaItemId` → `int`, FK → `MediaItem` (Movie or Episode only — enforced in service layer)
- `MediaItem` → `MediaItem`, navigation property
- `Progress` → `TimeSpan` (how far into the video the user got)
- `LastWatchedUtc` → `DateTime` (always store in UTC)
- `IsFinished` → `bool`

**Notes:**
> - `MediaItemId` points to the `BaseItems` table at the DB level (TPH) — EF can't enforce that it's only `Movie` or `Episode` at the DB level, so validate this in the service layer
> - Use `TimeSpan` for `Progress` — EF maps it to SQL `time` column. Max value is 23:59:59 which is fine for any movie or episode
> - `LastWatchedUtc` — always assign `DateTime.UtcNow` when writing, never `DateTime.Now`
> - After creating this file, go back to `Profile.cs` (Task 3) and remove the `// TODO` comment — the `WatchHistory` collection is now wired
> - Namespace: `Netflix_clone.Models.History`

**Done when:** Class has all 8 properties, `Progress` is `TimeSpan`, `LastWatchedUtc` is `DateTime`, compiles clean.

---

## Task 5 — `ApplicationDbContext.cs`

**File:** `Data/ApplicationDbContext.cs`

The EF Core database context — the single source of truth for the schema.

**DbSets to include:**
- `BaseItems` → `DbSet<BaseItem>` (TPH root — covers all subtypes)
- `Seasons` → `DbSet<Season>`
- `Actors` → `DbSet<Actor>`
- `Categories` → `DbSet<Category>`
- `Profiles` → `DbSet<Profile>`
- `WatchHistory` → `DbSet<WatchHistory>`

**Inherit from:** `IdentityDbContext<AppUser, AppRole, string>`

**`OnModelCreating` configuration to include:**

1. **TPH discriminator** — EF handles this automatically for `BaseItem` subtypes, but call `base.OnModelCreating(modelBuilder)` first or Identity tables won't be created

2. **Rating precision** — configure `decimal(4,1)` on `BaseItem.Rating`:
   ```csharp
   modelBuilder.Entity<BaseItem>()
       .Property(b => b.Rating)
       .HasPrecision(4, 1);
   ```

3. **Junction table: GeneralSeriesActors**
   ```csharp
   modelBuilder.Entity<GeneralSeries>()
       .HasMany(gs => gs.Actors)
       .WithMany()
       .UsingEntity(j => j.ToTable("GeneralSeriesActors"));
   ```

4. **Junction table: GeneralSeriesCategories**
   ```csharp
   modelBuilder.Entity<GeneralSeries>()
       .HasMany(gs => gs.Categories)
       .WithMany()
       .UsingEntity(j => j.ToTable("GeneralSeriesCategories"));
   ```

5. **Junction table: BaseItemActors** (standalone Movie actors)
   ```csharp
   modelBuilder.Entity<Movie>()
       .HasMany(m => m.Actors)
       .WithMany()
       .UsingEntity(j => j.ToTable("BaseItemActors"));
   ```

6. **Junction table: BaseItemCategories** (standalone Movie categories)
   ```csharp
   modelBuilder.Entity<Movie>()
       .HasMany(m => m.Categories)
       .WithMany()
       .UsingEntity(j => j.ToTable("BaseItemCategories"));
   ```

7. **Season → Series FK** (explicit, since Series is in the TPH table):
   ```csharp
   modelBuilder.Entity<Season>()
       .HasOne(s => s.Series)
       .WithMany(s => s.Seasons)
       .HasForeignKey(s => s.SeriesId);
   ```

8. **WatchHistory → MediaItem FK**:
   ```csharp
   modelBuilder.Entity<WatchHistory>()
       .HasOne(wh => wh.MediaItem)
       .WithMany()
       .HasForeignKey(wh => wh.MediaItemId);
   ```

**Notes:**
> - Always call `base.OnModelCreating(modelBuilder)` as the **first line** inside `OnModelCreating` — skipping it breaks all Identity tables
> - Do NOT add `[Table]` attributes to models — all table naming is done here via Fluent API
> - The junction table names must match the design doc exactly — casing matters
> - Namespace: `Netflix_clone.Data`

**Done when:** Context inherits `IdentityDbContext<AppUser, AppRole, string>`, has all 6 DbSets, all 8 Fluent API configurations are present, compiles clean.

---

## Task 6 — Register DbContext & Identity in `Program.cs`

**File:** `Program.cs`

Wire up EF Core and Identity so the app can connect to the database.

**Changes to make:**

1. Add the connection string to `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NetflixCloneDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

2. Register `ApplicationDbContext` in `Program.cs`:
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

3. Register Identity:
   ```csharp
   builder.Services.AddIdentity<AppUser, AppRole>(options =>
   {
       options.Password.RequireDigit = true;
       options.Password.RequiredLength = 8;
       options.Password.RequireNonAlphanumeric = false;
   })
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();
   ```

4. Add `app.UseAuthentication()` **before** `app.UseAuthorization()` in the middleware pipeline.

**Notes:**
> - `UseSqlServer` comes from `Microsoft.EntityFrameworkCore.SqlServer` — make sure Task 1 is done first
> - `app.UseAuthentication()` must come before `app.UseAuthorization()` — wrong order causes silent auth failures
> - The connection string uses `localdb` for local dev — coordinate with the team if using a shared SQL Server instance instead
> - Add the necessary `using` statements at the top of `Program.cs`

**Done when:** App builds and runs without errors, DbContext and Identity are registered in DI.

---

## Task 7 — Initial EF Core Migration

**No file — run these commands in the project root.**

Create the initial migration that captures the full schema:

```bash
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
```

Then review the generated migration file before applying it:
- Confirm `BaseItems` table exists with a `Discriminator` column
- Confirm `Seasons` table exists with `SeriesId` FK
- Confirm all 4 junction tables exist: `GeneralSeriesActors`, `GeneralSeriesCategories`, `BaseItemActors`, `BaseItemCategories`
- Confirm all Identity tables exist: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.
- Confirm `WatchHistory` table exists with `ProfileId` and `MediaItemId` FKs

**Notes:**
> - If the migration file looks wrong (missing tables, wrong column types), do NOT apply it — fix `ApplicationDbContext.cs` and run `dotnet ef migrations remove` then re-add
> - The `--output-dir Data/Migrations` flag keeps migration files organized under the `Data/` folder
> - Do NOT edit the generated migration file manually unless you know exactly what you're doing

**Done when:** Migration file exists under `Data/Migrations/`, reviewed and looks correct.

---

## Task 8 — Apply Migration

**No file — run this command in the project root.**

Apply the migration to the local dev database:

```bash
dotnet ef database update
```

**Notes:**
> - This creates the database if it doesn't exist yet (localdb will create it automatically)
> - If it fails, check the connection string in `appsettings.json` first
> - After applying, open SQL Server Object Explorer (or use `sqlcmd`) and verify the tables were created
> - If you need to start over: `dotnet ef database drop` then `dotnet ef database update`

**Done when:** Database exists, all tables are present, no migration errors.

---

## Task 9 — `SeedData.cs`

**File:** `Data/Seed/SeedData.cs`

Seeds the minimum data needed for the rest of the team to start working.

**Seed the following:**

1. **Categories** (at least 4):
   - "Action", "Drama", "Comedy", "Thriller"

2. **One Actor** (placeholder):
   - Name: "Sample Actor", Bio: null, Photo: null

3. **One Admin user** via `UserManager` + `RoleManager`:
   - Role: `"Admin"` — create it if it doesn't exist
   - User: email `admin@watchit.com`, password `Admin@1234`
   - Assign the `"Admin"` role to the user

**Structure:**

```csharp
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // get scoped services: ApplicationDbContext, UserManager<AppUser>, RoleManager<AppRole>
        // seed categories
        // seed actor
        // seed admin role
        // seed admin user
        // seed role assignment
    }
}
```

**Call it from `Program.cs`** after `app.Build()` and before `app.Run()`:
```csharp
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}
```

**Notes:**
> - Always check if data already exists before inserting — use `if (!context.Categories.Any())` guards to make seeding idempotent (safe to run multiple times)
> - Use `UserManager.CreateAsync(user, password)` not direct DB insert — Identity hashes the password for you
> - Use `RoleManager.RoleExistsAsync(roleName)` before creating a role
> - `await context.SaveChangesAsync()` after each logical group (categories, actors) — don't batch everything into one save
> - Namespace: `Netflix_clone.Data.Seed`

**Done when:** App starts, seed runs without errors, database contains the 4 categories, 1 actor, and 1 admin user with the Admin role assigned.

---

## Final Checklist Before Handing Off to Member 3

- [ ] `Models/Identity/AppUser.cs` and `AppRole.cs` exist and compile
- [ ] `Models/Profiles/Profile.cs` exists with `AppUserId` as `string`
- [ ] `Models/History/WatchHistory.cs` exists with `TimeSpan Progress` and `DateTime LastWatchedUtc`
- [ ] `Data/ApplicationDbContext.cs` inherits `IdentityDbContext<AppUser, AppRole, string>`
- [ ] All 4 junction tables are named correctly in Fluent API: `GeneralSeriesActors`, `GeneralSeriesCategories`, `BaseItemActors`, `BaseItemCategories`
- [ ] `base.OnModelCreating(modelBuilder)` is the first call in `OnModelCreating`
- [ ] `Rating` is configured as `decimal(4,1)` in Fluent API
- [ ] `Program.cs` registers DbContext, Identity, and has `UseAuthentication()` before `UseAuthorization()`
- [ ] Migration exists under `Data/Migrations/` and has been reviewed
- [ ] `dotnet ef database update` ran successfully — database exists with all tables
- [ ] Seed data is present: 4 categories, 1 actor, 1 admin user with Admin role
- [ ] `dotnet build` — zero errors, zero warnings
- [ ] Member 3 has the connection string and can connect to the dev database
