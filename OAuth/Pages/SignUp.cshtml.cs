using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OAuth.Data;
using OAuth.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthContext _authContext;
        public LoginModel(AuthContext authContext)
        {
            _authContext = authContext;
        }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [MinLength(6, ErrorMessage = "Password too short, min length 6 characters"),
            MaxLength(18, ErrorMessage = "Password too long, max length 20 characters")]
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } ="red";
        public string ReturnUrl { get; set; } = string.Empty;
        

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost(string email, string password, string confirmPassword)
        {
            var user = await _authContext.AuthUsers.FirstOrDefaultAsync(x=>x.Email == email);

            if (user != null)
            {
                Status = $"User with email: {email}, already exist!";
                return Page();
            }
            if (password == null)
            {
                Status = "The password field must not be empty";
                return Page();
            }
            if (password.Length < 6 || password.Length > 20)
            {
                Status = "Password must be more then 6 characters and less then 20 characters";
                return Page();
            }
            if (password != confirmPassword)
            {
                Status = "Password don`t match";
                return Page();
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var userToRegistration = new AuthUser
            {
                Email = email,
                PasswordHash = passwordHash,
                RoleId = 1
            };


            await _authContext.AuthUsers.AddAsync(userToRegistration);
            await _authContext.SaveChangesAsync();

            StatusColor = "mediumspringgreen";
            Status = "Registration is successesful!";
           
            return Page();
        }


    }
}
