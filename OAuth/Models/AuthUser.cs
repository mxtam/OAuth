using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        [MaxLength(10)]
        public string? Language { get; set; }
    }
}
