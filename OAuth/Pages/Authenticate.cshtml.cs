using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using OAuth.Data;
using Microsoft.EntityFrameworkCore;

namespace OAuth.Pages;

public class AuthenticateModel : PageModel
{
    private readonly AuthContext _authContext;
    public AuthenticateModel(AuthContext authContext) 
    { 
        _authContext = authContext;
    }
        public string Email { get; set; } = Consts.Email;

        public string Password { get; set; } = Consts.Password;

        [BindProperty]
        public string? ReturnUrl { get; set; }
        public string StatusColor { get; set; } = "red";
        public string AuthStatus { get; set; } = "";


        public IActionResult OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            //Шукаємо користувача за електронною поштою в БД
            var user = await _authContext.AuthUsers.FirstOrDefaultAsync(x=>x.Email == email);

            //Перевірка на правильність введення даних користувачем
            if (user == null)
            {
                AuthStatus = "Incorrect email address";
                return Page();
            }
            if (password == null)
            {
                AuthStatus = "The password field must not be empty";
                return Page();
            }

            //Порівнюємо введений пароль з збереженим хешем паролю в БД 
            var verifiedPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (verifiedPassword == false)
            {
                AuthStatus = "Password is invalid";
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
            };

            var principal = new ClaimsPrincipal(
                  new List<ClaimsIdentity>
                  {
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                  });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            StatusColor = "mediumspringgreen";
            AuthStatus = "Successfully authenticated";
            return Page();
        
        }
}
