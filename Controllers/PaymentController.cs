using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netflix_clone.Models;
using Netflix_clone.Repositories;
using Stripe;
using Stripe.Checkout;
using AppSubscription = Netflix_clone.Models.Subscription;

namespace Netflix_clone.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IGenericRepository<AppSubscription> _subscriptionRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        private static readonly Dictionary<string, (string Name, long PriceInCents, string Description, SubscriptionPlan Plan)> PlanOptions = new()
        {
            ["basic"]    = ("Basic",    799,  "HD streaming · 1 screen",          SubscriptionPlan.Basic),
            ["standard"] = ("Standard", 1299, "Full HD streaming · 2 screens",    SubscriptionPlan.Standard),
            ["premium"]  = ("Premium",  1799, "4K + HDR streaming · 4 screens",   SubscriptionPlan.Premium),
        };

        public PaymentController(IGenericRepository<AppSubscription> subscriptionRepo, UserManager<AppUser> userManager, IConfiguration config)
        {
            _subscriptionRepo = subscriptionRepo;
            _userManager = userManager;
            _config      = config;
        }

        [AllowAnonymous]
        [HttpGet("/Subscription")]
        public IActionResult Plans()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession(string planKey)
        {
            if (!PlanOptions.TryGetValue(planKey.ToLower(), out var plan))
                return BadRequest("Invalid plan.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user   = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var existing = _subscriptionRepo.Find(s => s.AppUserId == userId && s.Status == SubscriptionStatus.Active);
            if (existing != null)
                return RedirectToAction(nameof(Success));

            var customerService = new CustomerService();
            Customer customer;

            var existingAny = _subscriptionRepo.Find(s => s.AppUserId == userId && !string.IsNullOrEmpty(s.StripeCustomerId));

            if (existingAny != null)
            {
                customer = await customerService.GetAsync(existingAny.StripeCustomerId);
            }
            else
            {
                customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name  = user.UserName,
                    Metadata = new Dictionary<string, string> { ["userId"] = userId }
                });
            }

            var priceService = new PriceService();
            var price = await priceService.CreateAsync(new PriceCreateOptions
            {
                UnitAmount = plan.PriceInCents,
                Currency   = "usd",
                Recurring  = new PriceRecurringOptions { Interval = "month" },
                ProductData = new PriceProductDataOptions { Name = $"WatchIt {plan.Name}" }
            });

            var domain = $"{Request.Scheme}://{Request.Host}";
            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(new SessionCreateOptions
            {
                Customer           = customer.Id,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions { Price = price.Id, Quantity = 1 }
                },
                Mode       = "subscription",
                SuccessUrl = $"{domain}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl  = $"{domain}/Payment/Cancel",
                Metadata   = new Dictionary<string, string>
                {
                    ["userId"]  = userId,
                    ["planKey"] = planKey.ToLower()
                }
            });

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success([FromQuery(Name = "session_id")] string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(sessionId);

                if (session.PaymentStatus == "paid" || session.Status == "complete")
                {
                    var userId = session.Metadata.TryGetValue("userId", out var uid) ? uid : null;
                    var planKey = session.Metadata.TryGetValue("planKey", out var pk) ? pk : null;

                    if (userId != null && planKey != null && PlanOptions.TryGetValue(planKey, out var plan))
                    {
                        var existing = _subscriptionRepo.Find(s => s.AppUserId == userId);

                        if (existing == null)
                        {
                            _subscriptionRepo.Add(new AppSubscription
                            {
                                AppUserId             = userId,
                                Plan                  = plan.Plan,
                                Status                = SubscriptionStatus.Active,
                                StripeCustomerId      = session.CustomerId ?? string.Empty,
                                StripeSubscriptionId  = session.SubscriptionId ?? string.Empty,
                                StartDate             = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            existing.Plan                 = plan.Plan;
                            existing.Status               = SubscriptionStatus.Active;
                            existing.StripeCustomerId     = session.CustomerId ?? existing.StripeCustomerId;
                            existing.StripeSubscriptionId = session.SubscriptionId ?? existing.StripeSubscriptionId;
                            existing.StartDate            = DateTime.UtcNow;
                            existing.EndDate              = null;
                        }

                        await _subscriptionRepo.SaveAsync();
                    }
                }
            }

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Webhook()
        {
            var webhookSecret = _config["Stripe:WebhookSecret"];
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                if (stripeEvent.Type == "customer.subscription.deleted" ||
                    stripeEvent.Type == "customer.subscription.updated")
                {
                    var stripeSub = stripeEvent.Data.Object as Stripe.Subscription;
                    if (stripeSub != null)
                    {
                        var sub = _subscriptionRepo.Find(s => s.StripeSubscriptionId == stripeSub.Id);

                        if (sub != null)
                        {
                            sub.Status  = stripeSub.Status == "active"
                                ? SubscriptionStatus.Active
                                : SubscriptionStatus.Cancelled;
                            sub.EndDate = stripeSub.CanceledAt ?? stripeSub.CurrentPeriodEnd;
                            await _subscriptionRepo.SaveAsync();
                        }
                    }
                }
                return Ok();
            }
            catch (StripeException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sub = _subscriptionRepo.Find(s => s.AppUserId == userId && s.Status == SubscriptionStatus.Active);

            if (sub == null)
                return RedirectToAction(nameof(Plans));

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
            {
                var service = new SubscriptionService();
                await service.CancelAsync(sub.StripeSubscriptionId);
            }

            sub.Status  = SubscriptionStatus.Cancelled;
            sub.EndDate = DateTime.UtcNow;
            await _subscriptionRepo.SaveAsync();

            return RedirectToAction(nameof(Plans));
        }
    }
}
