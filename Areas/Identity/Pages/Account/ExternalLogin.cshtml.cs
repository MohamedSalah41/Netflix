#nullable disable

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Netflix_clone.Models;

namespace Netflix_clone.Areas.Identity.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(256, MinimumLength = 1)]
            [Display(Name = "Username")]
            public string UserName { get; set; }
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(provider))
            {
                ErrorMessage = "Invalid external login provider.";
                return RedirectToPage("./Login");
            }

            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            if (result.IsNotAllowed)
            {
                ErrorMessage = "External login is not allowed for this account.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                ErrorMessage = "Email claim not received from Google. Cannot create account.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            Input = new InputModel { Email = email };
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (!ModelState.IsValid)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }

            var user = new AppUser();
            await _userManager.SetUserNameAsync(user, Input.UserName);
            await _userManager.SetEmailAsync(user, Input.Email);

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var error in addLoginResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
    }
}
