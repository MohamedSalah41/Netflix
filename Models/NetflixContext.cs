using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Netflix_clone.Models
{
    public class NetflixContext : IdentityDbContext<AppUser>
    {
        public NetflixContext(DbContextOptions<NetflixContext> options)
            : base(options) {}

        // ── DbSets ───────────────────────────────────────────────────────────
        public DbSet<Series> Series => Set<Series>();
        public DbSet<SeriesOfMovies> SeriesOfMovies => Set<SeriesOfMovies>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Episode> Episodes => Set<Episode>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<WatchHistory> WatchHistory => Set<WatchHistory>();
        public DbSet<MyListItem> MyListItems => Set<MyListItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================================================================
            // TPC — Table Per Concrete Type
            // Each concrete class gets its own table; abstract classes have none.
            // ================================================================

            modelBuilder.Entity<BaseItem>().UseTpcMappingStrategy();

            // ================================================================
            // BaseItem — shared column config (inherited by all concrete tables)
            // ================================================================

            modelBuilder.Entity<BaseItem>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Id)
                      .UseHiLo("BaseItemIdSequence"); // TPC needs a shared key strategy

                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(b => b.Description)
                      .HasMaxLength(2000);

                entity.Property(b => b.Poster)
                      .HasMaxLength(512);

                entity.Property(b => b.Rating)
                      .HasPrecision(4, 1)
                      .HasDefaultValue(0.0m);
            });

            // ================================================================
            // GeneralSeries — abstract, no table; column config only.
            // Actors/Categories live on each concrete subtype (Series, SeriesOfMovies)
            // with their own junction tables — TPC cannot configure many-to-many
            // on abstract types.
            // ================================================================

            modelBuilder.Entity<GeneralSeries>(entity =>
            {
                entity.Property(g => g.TrailerUrl)
                      .HasMaxLength(1024);
            });

            // ================================================================
            // MediaItem — abstract, no table; column config only
            // ================================================================

            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.Property(m => m.VideoUrl)
                      .IsRequired()
                      .HasMaxLength(1024);
            });

            // ================================================================
            // Series → concrete table
            // ================================================================

            modelBuilder.Entity<Series>(entity =>
            {
                entity.ToTable("Series");

                entity.HasMany(s => s.Seasons)
                      .WithOne(s => s.Series)
                      .HasForeignKey(s => s.SeriesId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Series <-> Actors
                entity.HasMany(s => s.Actors)
                      .WithMany()
                      .UsingEntity(
                          "SeriesActors",
                          r => r.HasOne(typeof(Actor)).WithMany().HasForeignKey("ActorId"),
                          l => l.HasOne(typeof(Series)).WithMany().HasForeignKey("SeriesId")
                      );

                // Series <-> Categories
                entity.HasMany(s => s.Categories)
                      .WithMany()
                      .UsingEntity(
                          "SeriesCategories",
                          r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId"),
                          l => l.HasOne(typeof(Series)).WithMany().HasForeignKey("SeriesId")
                      );
            });

            // ================================================================
            // SeriesOfMovies → concrete table
            // ================================================================

            modelBuilder.Entity<SeriesOfMovies>(entity =>
            {
                entity.ToTable("SeriesOfMovies");

                entity.HasMany(s => s.Movies)
                      .WithOne(m => m.SeriesOfMovies)
                      .HasForeignKey(m => m.SeriesOfMoviesId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                // SeriesOfMovies <-> Actors  (reuse GeneralSeriesActors concept, separate junction)
                entity.HasMany(s => s.Actors)
                      .WithMany()
                      .UsingEntity(
                          "SeriesOfMoviesActors",
                          r => r.HasOne(typeof(Actor)).WithMany().HasForeignKey("ActorId"),
                          l => l.HasOne(typeof(SeriesOfMovies)).WithMany().HasForeignKey("SeriesOfMoviesId")
                      );

                // SeriesOfMovies <-> Categories
                entity.HasMany(s => s.Categories)
                      .WithMany()
                      .UsingEntity(
                          "SeriesOfMoviesCategories",
                          r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId"),
                          l => l.HasOne(typeof(SeriesOfMovies)).WithMany().HasForeignKey("SeriesOfMoviesId")
                      );
            });

            // ================================================================
            // Movie → concrete table
            // ================================================================

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("Movies");

                entity.Property(m => m.SeriesOfMoviesId)
                      .IsRequired(false);

                // Movie <-> Actors
                entity.HasMany(m => m.Actors)
                      .WithMany()
                      .UsingEntity(
                          "MovieActors",
                          r => r.HasOne(typeof(Actor)).WithMany().HasForeignKey("ActorId"),
                          l => l.HasOne(typeof(Movie)).WithMany().HasForeignKey("MovieId")
                      );

                // Movie <-> Categories
                entity.HasMany(m => m.Categories)
                      .WithMany()
                      .UsingEntity(
                          "MovieCategories",
                          r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId"),
                          l => l.HasOne(typeof(Movie)).WithMany().HasForeignKey("MovieId")
                      );
            });

            // ================================================================
            // Episode → concrete table
            // ================================================================

            modelBuilder.Entity<Episode>(entity =>
            {
                entity.ToTable("Episodes");

                entity.Property(e => e.Number)
                      .IsRequired();

                entity.Property(e => e.SeasonId)
                      .IsRequired(false);

                entity.HasOne(e => e.Season)
                      .WithMany(s => s.Episodes)
                      .HasForeignKey(e => e.SeasonId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================================================================
            // Season — standalone table (not in TPH/TPC tree)
            // ================================================================

            modelBuilder.Entity<Season>(entity =>
            {
                entity.ToTable("Seasons");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(s => s.Number)
                      .IsRequired();

                entity.Property(s => s.Poster)
                      .HasMaxLength(512);

                entity.Property(s => s.SeriesId)
                      .IsRequired(false);

                // Season -> Series relationship is configured from Series side above
            });

            // ================================================================
            // Actor
            // ================================================================

            modelBuilder.Entity<Actor>(entity =>
            {
                entity.ToTable("Actors");

                entity.HasKey(a => a.Id);

                entity.Property(a => a.Name)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(a => a.Bio)
                      .HasMaxLength(2000);

                entity.Property(a => a.Photo)
                      .HasMaxLength(512);
            });

            // ================================================================
            // Category
            // ================================================================

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(128);
            });

            // ================================================================
            // Profile
            // ================================================================

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("Profiles");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(p => p.Image)
                      .HasMaxLength(512);

                entity.Property(p => p.AppUserId)
                      .IsRequired(false);

                entity.HasOne(p => p.AppUser)
                      .WithMany(u => u.Profiles)
                      .HasForeignKey(p => p.AppUserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================================================================
            // WatchHistory
            // ================================================================

            modelBuilder.Entity<WatchHistory>(entity =>
            {
                entity.ToTable("WatchHistory");

                entity.HasKey(w => w.Id);

                entity.Property(w => w.LastWatchedUtc)
                      .IsRequired();

                entity.Property(w => w.IsFinished)
                      .HasDefaultValue(false);

                entity.Property(w => w.ProfileId)
                      .IsRequired(false);

                entity.Property(w => w.MediaItemId)
                      .IsRequired(false);

                entity.HasOne(w => w.Profile)
                      .WithMany(p => p.WatchHistory)
                      .HasForeignKey(w => w.ProfileId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                // MediaItemId stores the PK of either a Movie or Episode row.
                // Resolved in the service layer — no FK constraint possible with TPC
                // since Movie and Episode live in separate tables.
                entity.Property(w => w.MediaItemId)
                      .IsRequired(false);
            });

            modelBuilder.Entity<MyListItem>(entity =>
            {
                entity.ToTable("MyListItems");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.AppUserId).IsRequired();
                entity.Property(m => m.MediaType).HasMaxLength(20).IsRequired();
                entity.Property(m => m.MediaName).HasMaxLength(256).IsRequired();
                entity.Property(m => m.MediaPoster).HasMaxLength(512);
                entity.HasOne(m => m.AppUser)
                      .WithMany()
                      .HasForeignKey(m => m.AppUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
