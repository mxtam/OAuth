using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ResourceServer.Controllers
{
    [ApiController]
    [Route("resources")]
    public class ResourceController:Controller
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSecretResources()
        {
            var lang = HttpContext.Response.Headers.ContentLanguage;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                         new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });


            var user = HttpContext.User?.Identity?.Name;

            if (lang == "uk-UA")
            {
                return Ok($"Привіт, {user}!");
            }

            return Ok($"Hello, {user}!");
        }
    }
}
