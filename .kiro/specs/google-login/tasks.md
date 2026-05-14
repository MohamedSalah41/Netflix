# Implementation Plan: Google Login

## Overview

Add Google OAuth 2.0 external login to the Netflix clone by wiring the Google authentication middleware, creating the `ExternalLogin` Razor Page, and updating the Login and Register views to surface provider buttons.

## Tasks

- [x] 1. Add NuGet package and configuration placeholder
  - [x] 1.1 Add `Microsoft.AspNetCore.Authentication.Google` package to `Netflix-clone.csproj`
    - Add `<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="10.0.0" />` inside the existing `<ItemGroup>` that holds other `PackageReference` entries
    - Run `dotnet restore` to confirm the package resolves
    - _Requirements: 1.1_

  - [x] 1.2 Add `Authentication:Google` placeholder section to `appsettings.json`
    - Insert the `"Authentication": { "Google": { "ClientId": "YOUR_GOOGLE_CLIENT_ID", "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET" } }` block at the top level of `appsettings.json`
    - _Requirements: 1.3_

- [x] 2. Register Google provider in `Program.cs`
  - [x] 2.1 Chain `.AddGoogle(...)` onto the existing `AddIdentity` call in `Program.cs`
    - Read `ClientId` and `ClientSecret` from `builder.Configuration["Authentication:Google:ClientId"]` and `builder.Configuration["Authentication:Google:ClientSecret"]`
    - Use the null-coalescing throw pattern (`?? throw new InvalidOperationException(...)`) for both values so the app fails fast at startup when either is missing
    - _Requirements: 1.1, 1.2, 1.3_

- [x] 3. Create `ExternalLogin` Razor Page model
  - [x] 3.1 Create `Areas/Identity/Pages/Account/ExternalLogin.cshtml.cs`
    - Declare `ExternalLoginModel : PageModel` in namespace `Netflix_clone.Areas.Identity.Pages.Account`
    - Inject `SignInManager<AppUser>`, `UserManager<AppUser>`, and `ILogger<ExternalLoginModel>`
    - Declare `InputModel` with `[Required][EmailAddress] Email` and `[Required][StringLength(256, MinimumLength = 1)] UserName` properties
    - Expose `ProviderDisplayName`, `ReturnUrl` (string), and `[TempData] ErrorMessage` properties
    - Implement `OnPost(string provider, string returnUrl)`: guard null/empty provider → redirect to Login with error; otherwise build `AuthenticationProperties` with callback URL set to `Url.Page("./ExternalLogin", pageHandler: "Callback", ...)` and return `ChallengeResult`
    - Implement `OnGetCallbackAsync(string returnUrl, string remoteError)`: handle `remoteError` → redirect to Login; handle null `ExternalLoginInfo` → redirect to Login; call `ExternalLoginSignInAsync` → on success redirect to `returnUrl`; on lockout redirect to Lockout page; on failure/not-allowed redirect to Login with error; if no existing login extract email claim (null → redirect to Login with error), set `Input.Email`, set `ProviderDisplayName`, return `Page()`
    - Implement `OnPostConfirmationAsync(string returnUrl)`: re-fetch `ExternalLoginInfo` (null → redirect to Login); validate `ModelState`; create `AppUser` via `UserManager.CreateAsync` (errors → redisplay); call `UserManager.AddLoginAsync` (failure → delete user, redisplay); call `SignInManager.SignInAsync`; redirect to `returnUrl`
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 6.1, 6.2, 6.3, 6.4, 6.5, 7.1_

- [x] 4. Create `ExternalLogin` Razor Page view
  - [x] 4.1 Create `Areas/Identity/Pages/Account/ExternalLogin.cshtml`
    - Set `@page "/Account/ExternalLogin"` and `@model ExternalLoginModel`
    - Include `<partial name="_MovieFormStyle" />` to match the existing page style
    - Render a `<form asp-page-handler="Confirmation" asp-route-returnUrl="@Model.ReturnUrl" method="post">` containing a read-only email input bound to `Input.Email` and an editable username input bound to `Input.UserName` with `asp-validation-for`
    - Include `<div asp-validation-summary="ModelOnly" ...>` for model-level errors
    - Include `<partial name="_ValidationScriptsPartial" />` in the `Scripts` section
    - _Requirements: 5.1, 5.3, 5.7, 5.8, 6.5_

- [x] 5. Update `Login.cshtml` to render external provider buttons
  - [x] 5.1 Add external login section to `Areas/Identity/Pages/Account/Login.cshtml`
    - Insert the `@if (Model.ExternalLogins?.Count > 0)` block after the closing `</form>` of the password form and before the closing `</div>` of `.form-card`
    - The block must contain a `<form asp-area="Identity" asp-page="/Account/ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post">` with a `@foreach` loop rendering one `<button type="submit" name="provider" value="@provider.Name">Sign in with @provider.DisplayName</button>` per provider
    - When `ExternalLogins` is null or empty the section must not render
    - _Requirements: 2.1, 2.2, 2.4_

- [x] 6. Update `Register.cshtml` to render external provider buttons
  - [x] 6.1 Add external login section to `Areas/Identity/Pages/Account/Register.cshtml`
    - Insert the `@if (Model.ExternalLogins?.Count > 0)` block after the closing `</form>` of the registration form and before the closing `</div>` of `.form-card`
    - The block must contain a `<form asp-area="Identity" asp-page="/Account/ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post">` with a `@foreach` loop rendering one `<button type="submit" name="provider" value="@provider.Name">Sign up with @provider.DisplayName</button>` per provider
    - When `ExternalLogins` is null or empty the section must not render
    - _Requirements: 3.1, 3.2, 3.3_

- [x] 7. Build verification checkpoint
  - Run `dotnet build` and confirm zero errors and zero warnings related to the new files
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Real Google credentials must be supplied via environment variables or user secrets before the app can complete an OAuth round-trip; the placeholder values in `appsettings.json` will cause a startup exception by design
- The `ExternalLogins` property is already populated in both `LoginModel.OnGetAsync` and `RegisterModel.OnGetAsync`, so no changes to those page models are needed
- `LocalRedirect(returnUrl)` is used throughout to prevent open redirect attacks

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1", "1.2"] },
    { "id": 1, "tasks": ["2.1"] },
    { "id": 2, "tasks": ["3.1"] },
    { "id": 3, "tasks": ["4.1", "5.1", "6.1"] }
  ]
}
```
