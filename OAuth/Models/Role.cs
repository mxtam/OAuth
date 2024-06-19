using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string RoleName { get; set; } = string.Empty;
        public List<AuthUser>? authUsers { get; set; }
    }
}
