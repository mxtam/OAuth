using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Web;
using OAuth.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Cryptography;
using OAuth.Data;
using OAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

namespace OAuth.Controllers
{
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly AuthorizationService _authService;
        private readonly AuthContext _authContext;

        public AuthorizationController(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            AuthorizationService authService,
            AuthContext authContext)
        {
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
            _authService = authService;
            _authContext = authContext;
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        public async Task<IActionResult> Authorize()
        {
            //Перевірка правильності запиту Open Id Connect
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

           //Перевіряємо чи на сервері авторизації існує клієнт з Id з запиту
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                              throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            //Отримуємо мову користувача
            var lang = HttpContext.Response.Headers.ContentLanguage;
            //Додаємо її до кукі "language"
            Response.Cookies.Append("language",lang,
                         new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            //Зберігаємо codeChallenge та codeMethod з запиту
            var codeChallenge = request.CodeChallenge;
            var codeChallengeMethod = request.CodeChallengeMethod;

            //Якщо клієнт серверу авторизації має тип не Explicit повертаємо помилку
            if (await _applicationManager.GetConsentTypeAsync(application) != ConsentTypes.Explicit)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidClient,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Only clients with explicit consent type are allowed."
                    }));
            }

           //Парсимо параметри з рядку запиту
            var parameters = _authService.ParseOAuthParameters(HttpContext, new List<string> { Parameters.Prompt });

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

           //Перевіряємо чи користувач автентифікований, якщо ні, повертаємо його на сторінку автентифікації 
            if (!_authService.IsAuthenticated(result, request))
            {
                return Challenge(properties: new AuthenticationProperties
                {
                    RedirectUri = _authService.BuildRedirectUrl(HttpContext.Request, parameters)
                }, new[] { CookieAuthenticationDefaults.AuthenticationScheme });
            }

            if (request.HasPrompt(Prompts.Login))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Challenge(properties: new AuthenticationProperties
                {
                    RedirectUri = _authService.BuildRedirectUrl(HttpContext.Request, parameters)
                }, new[] { CookieAuthenticationDefaults.AuthenticationScheme });
            }

            var consentClaim = result.Principal.GetClaim(Consts.ConsentNaming);

            //Перевіряємо, якщо користувач відхилив доступ для автентифікації 
            //ми виходимо з його запису та перенаправляємо на сторінку автентифікації
            if (consentClaim == Consts.DenyAccessValue || request.HasPrompt(Prompts.Consent))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var returnUrl = HttpUtility.UrlEncode(_authService.BuildRedirectUrl(HttpContext.Request, parameters));
                var consentRedirectUrl = $"/Consent?returnUrl={returnUrl}";

                return Redirect(consentRedirectUrl);
            }
            
            if (consentClaim is null || request.HasPrompt(Prompts.Consent))
            {

                var returnUrl = HttpUtility.UrlEncode(_authService.BuildRedirectUrl(HttpContext.Request, parameters));
                var consentRedirectUrl = $"/Consent?returnUrl={returnUrl}";

                return Redirect(consentRedirectUrl);
            }

            var userId = result.Principal.FindFirst(ClaimTypes.Email)!.Value;

            //Встановлюємо клейми для користувача
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetClaim(Claims.Subject, userId)
                .SetClaim(Claims.Email, userId)
                .SetClaim(Claims.Name, userId)
                .SetClaims(Claims.Role, new List<string> { "User", "Admin" }.ToImmutableArray());

            identity.SetScopes(request.GetScopes());
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

            // Зберігаємо codeChallenge та codeMethod до БД
            var authCodeChallenge = new AuthorizationCodeChallenge
            {
                CodeChallenge = codeChallenge,
                CodeChallengeMethod = codeChallengeMethod,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                UserLanguage = lang
            };
            await _authContext.AuthCodeChallenge.AddAsync(authCodeChallenge);

            //Знаходимо нашого користувача якщо він існує та оновлюємо для нього мову в БД
            if (userId != null) 
            {
                var authUser = await _authContext.AuthUsers.FirstOrDefaultAsync(x => x.Email == userId);
                if (authUser != null) 
                {
                    authUser.Language = lang;
                    _authContext.AuthUsers.Update(authUser);
                }
            }

            //Зберігаємо зміни в БД
            await _authContext.SaveChangesAsync();

            //HttpContext.Session.SetString("codeChallenge", codeChallenge);
            //HttpContext.Session.SetString("codeChallengeMethod", codeChallengeMethod);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
                throw new InvalidOperationException("The specified grant type is not supported.");

            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var userId = result.Principal.GetClaim(Claims.Subject);

            var codeChallenge = string.Empty;
            var codeChallengeMethod = string.Empty;

            //Дістаємо з БД наш codeChallenge та codeMethod та записуємо в змінні
            var authCodeChallenge = await _authContext.AuthCodeChallenge
                                                        .OrderByDescending(c => c.CreatedDate)
                                                            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (authCodeChallenge != null)
            {
                codeChallenge = authCodeChallenge.CodeChallenge;
                codeChallengeMethod = authCodeChallenge.CodeChallengeMethod;
            }

            //var codeChallenge = HttpContext.Session.GetString("codeChallenge");
            //var codeChallengeMethod = HttpContext.Session.GetString("codeChallengeMethod");


            if (request.IsAuthorizationCodeGrantType())
            {
                //Дістаємо codeVerifier з запиту
                var codeVerifier = request.CodeVerifier;
                //За дпомогою методу ValidateCodeVerfier перевіряємо наш codeVerifier, 
                //у випадку некоректно наданого codeVerifier повертаємо помилку
                if (!ValidateCodeVerifier(codeVerifier, codeChallenge, codeChallengeMethod))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid code verifier."
                        }));
                }
            }

            
            //Повертаємо помилку, якщо користувача не знайдено
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Cannot find user from the token."
                    }));
            }

            //Встановлюємо клейми для користувача
            var identity = new ClaimsIdentity(result.Principal.Claims,
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetClaim(Claims.Subject, userId)
                .SetClaim(Claims.Email, userId)
                .SetClaim(Claims.Name, userId)
                .SetClaims(Claims.Role, new List<string> { "user", "admin" }.ToImmutableArray());

            identity.SetDestinations(c => AuthorizationService.GetDestinations(identity, c));

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private bool ValidateCodeVerifier(string codeVerifier, string codeChallenge, string codeChallengeMethod)
        {
            if (string.IsNullOrEmpty(codeVerifier) || string.IsNullOrEmpty(codeChallenge) || string.IsNullOrEmpty(codeChallengeMethod))
            {
                return false;
            }

            if (codeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase))
            {
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                    var hashedVerifier = Base64UrlEncoder.Encode(hash);

                    return hashedVerifier == codeChallenge;
                }
            }

            return true;
        }
        //Ендпоінт для UserInfo
        //[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        //[HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
        //public async Task<IActionResult> Userinfo()
        //{
        //    if (User.GetClaim(Claims.Subject) != Consts.Email)
        //    {
        //        return Challenge(
        //            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
        //            properties: new AuthenticationProperties(new Dictionary<string, string?>
        //            {
        //                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
        //                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
        //                    "The specified access token is bound to an account that no longer exists."
        //            }));
        //    }

        //    var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        //    {
        //        // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
        //        [Claims.Subject] = Consts.Email
        //    };

        //    if (User.HasScope(Scopes.Email))
        //    {
        //        claims[Claims.Email] = Consts.Email;
        //    }

        //    return Ok(claims);
        //}

        [HttpGet("~/connect/logout")]
        [HttpPost("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }

    }

}
