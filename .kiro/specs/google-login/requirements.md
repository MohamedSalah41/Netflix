# Requirements Document

## Introduction

This feature adds Google OAuth 2.0 external login support to the Netflix clone application. Users will be able to sign in or register using their Google account as an alternative to the existing email/password flow. The feature integrates with the existing ASP.NET Identity setup, reusing the scaffolded Login and Register pages and the `AppUser` model.

## Glossary

- **Google_OAuth_Provider**: The ASP.NET Core authentication middleware that handles the Google OAuth 2.0 flow using `Microsoft.AspNetCore.Authentication.Google`.
- **ExternalLogin_Callback**: The Identity Razor Page handler (`ExternalLoginCallback`) that processes the response returned by Google after the user authenticates.
- **AppUser**: The application's custom `IdentityUser` subclass stored in the `NetflixContext` database.
- **SignInManager**: The ASP.NET Identity service responsible for signing users in and out, including external login flows.
- **UserManager**: The ASP.NET Identity service responsible for creating and managing `AppUser` records.
- **External_Login_Info**: The object returned by `SignInManager.GetExternalLoginInfoAsync()` containing the provider name, provider key, and claims from Google.
- **ExternalLoginConfirmation_Page**: The page shown to first-time Google users to confirm or supply a username before account creation.

---

## Requirements

### Requirement 1: Google OAuth Provider Registration

**User Story:** As a developer, I want the application to register Google as an external authentication provider, so that the OAuth flow can be initiated from the login and register pages.

#### Acceptance Criteria

1. THE application SHALL register Google as an external authentication provider using the `ClientId` and `ClientSecret` values read from application configuration.
2. WHEN the application starts and the `ClientId` or `ClientSecret` configuration value is missing, empty, or contains only whitespace, THE application SHALL fail to start and surface an error indicating which configuration value is absent.
3. THE application SHALL retrieve `ClientId` and `ClientSecret` exclusively from configuration (e.g., environment variables, secrets manager, or a configuration file), and SHALL NOT contain credential values as literals in source code.

---

### Requirement 2: Google Login Button on Login Page

**User Story:** As a user, I want to see a "Sign in with Google" button on the login page, so that I can authenticate using my Google account instead of entering a password.

#### Acceptance Criteria

1. WHEN the Login page loads and at least one external authentication scheme is registered, THE Login page SHALL render one button per registered external provider, labelled with the provider's display name.
2. WHEN a user clicks an external provider button, THE Login page SHALL submit a POST that initiates an authentication challenge for that provider, preserving the current `returnUrl`, and redirect the browser to the provider's authorization URL.
3. WHEN the authentication challenge cannot be initiated (e.g., provider misconfiguration), THE application SHALL redirect the user to the Login page and display an error message.
4. WHEN no external authentication schemes are registered, THE Login page SHALL NOT render the external login section.

---

### Requirement 3: Google Sign-Up Button on Register Page

**User Story:** As a new user, I want to see a "Sign up with Google" button on the register page, so that I can create an account without choosing a password.

#### Acceptance Criteria

1. WHEN the Register page loads and at least one external authentication scheme is registered, THE Register page SHALL render one button per registered external provider, labelled with the provider's display name.
2. WHEN a user clicks an external provider button on the Register page, THE Register page SHALL submit a POST that initiates an authentication challenge for that provider, preserving the current `returnUrl`, and redirect the browser to the provider's authorization URL.
3. WHEN no external authentication schemes are registered, THE Register page SHALL NOT render the external login section.

---

### Requirement 4: External Login Callback — Returning User

**User Story:** As a returning user who previously linked their Google account, I want to be signed in automatically after Google redirects back, so that I do not need to take any additional steps.

#### Acceptance Criteria

1. WHEN Google redirects back to the application with a valid authorization code and the `External_Login_Info` matches an existing `AppUser` external login record, THE `ExternalLogin_Callback` SHALL sign the user in using `SignInManager.ExternalLoginSignInAsync`.
2. WHEN sign-in succeeds, THE `ExternalLogin_Callback` SHALL redirect the user to the `returnUrl` if it is a valid local URL, or to the application home page if `returnUrl` is absent, empty, or not a local URL.
3. IF `SignInManager.ExternalLoginSignInAsync` returns a lockout result, THEN THE `ExternalLogin_Callback` SHALL redirect the user to the Lockout page.
4. IF `SignInManager.ExternalLoginSignInAsync` returns a failure or not-allowed result, THEN THE `ExternalLogin_Callback` SHALL redirect the user to the Login page and display an error message.
5. IF `SignInManager.GetExternalLoginInfoAsync()` returns null, THEN THE `ExternalLogin_Callback` SHALL redirect the user to the Login page with an error message indicating that the external login information could not be retrieved.

---

### Requirement 5: External Login Callback — New User Account Creation

**User Story:** As a first-time Google user, I want to be prompted to confirm my username before my account is created, so that my profile has a valid username in the system.

#### Acceptance Criteria

1. WHEN Google redirects back and no existing `AppUser` external login record matches the `External_Login_Info`, THE `ExternalLogin_Callback` SHALL display the `ExternalLoginConfirmation_Page` pre-populated with the email address from the Google claims.
2. IF the Google claims do not contain an email address, THEN THE `ExternalLogin_Callback` SHALL redirect the user to the Login page with an error message and SHALL NOT display the `ExternalLoginConfirmation_Page`.
3. WHEN a new user submits the `ExternalLoginConfirmation_Page` with a username that is between 1 and 256 non-whitespace characters and is unique across existing `AppUser` records, and with the email from Google claims, THE `ExternalLogin_Callback` SHALL create a new `AppUser` with that username and email using `UserManager.CreateAsync`.
4. WHEN `UserManager.CreateAsync` succeeds, THE `ExternalLogin_Callback` SHALL add the external login record via `UserManager.AddLoginAsync`.
5. IF `UserManager.AddLoginAsync` fails after a successful `UserManager.CreateAsync`, THEN THE `ExternalLogin_Callback` SHALL delete the newly created `AppUser` and display all error messages on the `ExternalLoginConfirmation_Page` without leaving a partial account.
6. WHEN both `UserManager.CreateAsync` and `UserManager.AddLoginAsync` succeed, THE `ExternalLogin_Callback` SHALL sign the user in and redirect to the `returnUrl` if it is a valid local URL, or to the application home page otherwise.
7. IF `UserManager.CreateAsync` returns errors, THEN THE `ExternalLogin_Callback` SHALL display all error messages on the `ExternalLoginConfirmation_Page` without creating a partial account.
8. WHEN a new user submits the `ExternalLoginConfirmation_Page` with a username that is empty, whitespace-only, longer than 256 characters, or already taken, THE `ExternalLogin_Callback` SHALL redisplay the form with validation errors and SHALL NOT create an `AppUser` record.

---

### Requirement 6: ExternalLogin Razor Page

**User Story:** As a developer, I want a dedicated `ExternalLogin` Razor Page to handle the OAuth challenge and callback, so that the external login flow is encapsulated and separate from the password login page.

#### Acceptance Criteria

1. WHEN the `OnPost` handler is invoked with a non-null, non-empty provider name, THE `ExternalLogin` Razor Page SHALL issue an `AuthenticationProperties` challenge for the specified provider and set the callback URL to `/Account/ExternalLogin`.
2. IF the `OnPost` handler is invoked with a null or empty provider name, THEN THE `ExternalLogin` Razor Page SHALL redirect to the Login page with an error message and SHALL NOT issue a challenge.
3. WHEN the `OnGetCallbackAsync` handler is invoked with a valid OAuth callback, THE `ExternalLogin` Razor Page SHALL retrieve `External_Login_Info` and route to sign-in or account creation as defined in Requirements 4 and 5.
4. WHEN the `OnPostConfirmationAsync` handler is invoked and `External_Login_Info` is no longer available in the session, THE `ExternalLogin` Razor Page SHALL redirect to the Login page with an error message and SHALL NOT attempt account creation.
5. WHEN the `OnPostConfirmationAsync` handler is invoked with valid `External_Login_Info` present, THE `ExternalLogin` Razor Page SHALL accept the username confirmation form and complete new-user account creation as defined in Requirement 5.

---

### Requirement 7: Error Handling for OAuth Failures

**User Story:** As a user, I want to see a clear error message when the Google login fails, so that I understand what went wrong and can try again.

#### Acceptance Criteria

1. WHEN Google returns an error response (e.g., user denied consent), THE `ExternalLogin_Callback` SHALL redirect to the Login page and display an error message in the page's error summary that states the external login was unsuccessful and instructs the user to try again or use a different sign-in method.
2. WHEN an unhandled exception occurs during the OAuth callback, THE application SHALL write the exception details to the application log as an error-level entry and redirect the user to the Login page with an error message that states login could not be completed due to an unexpected error, without exposing internal exception details to the user.
