# 📂 Project Structure

```bash
Netflix/
│
├── Controllers/                         # MVC Controllers
│   ├── HomeController.cs                # Landing & browse pages
│   ├── AuthController.cs                # Login / Register
│   ├── ProfileController.cs             # Profile selection & management
│   ├── BrowseController.cs              # Categories & filtering
│   ├── WatchController.cs               # Video player & streaming
│   ├── HistoryController.cs             # Continue watching system
│   ├── SubscriptionController.cs        # Stripe subscriptions
│   ├── RatingController.cs              # Ratings & reviews
│   └── MyListController.cs              # Watchlist system
│
├── Models/                              # Entity models
│   ├── Identity/
│   │   ├── AppUser.cs
│   │   └── AppRole.cs
│   │
│   ├── Profiles/
│   │   └── Profile.cs
│   │
│   ├── Media/
│   │   ├── BaseItem.cs                  # Shared properties
│   │   ├── MediaItem.cs                 # Playable content
│   │   ├── Movie.cs
│   │   ├── Episode.cs
│   │   ├── Season.cs
│   │   ├── Series.cs
│   │   ├── SeriesOfMovies.cs
│   │   ├── Actor.cs
│   │   └── Category.cs
│   │
│   ├── History/
│   │   └── WatchHistory.cs
│   │
│   ├── Ratings/
│   │   └── Rating.cs
│   │
│   └── Subscription/
│       ├── SubscriptionPlan.cs
│       ├── UserSubscription.cs
│       └── PaymentTransaction.cs
│
├── Data/                                # Database layer
│   ├── ApplicationDbContext.cs
│   ├── Configurations/                  # Fluent API configs
│   ├── Seed/                            # Seed data
│   └── Migrations/
│
├── Repositories/                        # Data access abstraction
│   ├── Interfaces/
│   └── Implementations/
│
├── Services/                            # Business logic
│   ├── AuthService.cs
│   ├── ProfileService.cs
│   ├── BrowseService.cs
│   ├── WatchHistoryService.cs
│   ├── RecommendationService.cs
│   ├── StripeService.cs
│   ├── RatingService.cs
│   └── UploadService.cs
│
├── DTOs/                                # Data transfer objects
│   ├── Auth/
│   ├── Media/
│   ├── Subscription/
│   └── Profile/
│
├── ViewModels/                          # Models sent to Razor views
│   ├── Home/
│   ├── Browse/
│   ├── Player/
│   └── Admin/
│
├── Views/                               # Razor Views
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Navbar.cshtml
│   │   └── _Footer.cshtml
│   │
│   ├── Home/
│   ├── Auth/
│   ├── Browse/
│   ├── Watch/
│   ├── Profile/
│   ├── Subscription/
│   └── Admin/
│
├── Areas/
│   └── Admin/                           # Protected admin panel
│       ├── Controllers/
│       ├── Views/
│       └── ViewModels/
│
├── wwwroot/                             # Static files
│   ├── css/
│   │   ├── site.css
│   │   ├── responsive.css
│   │   └── darkmode.css
│   │
│   ├── js/
│   │   ├── player.js
│   │   ├── search.js
│   │   └── navbar.js
│   │
│   ├── images/
│   │   ├── posters/
│   │   ├── profiles/
│   │   └── banners/
│   │
│   ├── uploads/
│   │   ├── trailers/
│   │   └── videos/
│   │
│   └── lib/
│
├── Helpers/                             # Helper utilities
│   ├── JwtHelper.cs
│   ├── FileHelper.cs
│   └── ExtensionMethods.cs
│
├── Middleware/                          # Custom middleware
│   ├── ExceptionMiddleware.cs
│   └── SubscriptionMiddleware.cs
│
├── Configurations/                      # App configurations
│   ├── StripeSettings.cs
│   ├── JwtSettings.cs
│   └── CloudSettings.cs
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── README.md
```
