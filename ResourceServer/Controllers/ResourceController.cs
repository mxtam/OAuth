using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            //Отримуємо користувача
            var user = HttpContext.User?.Identity?.Name;

            //Отримуємо кукі для мови з запиту 
            var lang = Request.Cookies["language"];

            if (lang == "uk-UA")
            {
                return Ok($"Привіт, {user}!");
            }

            return Ok($"Hello, {user}!");
        }
    }
}
