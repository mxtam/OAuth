using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
