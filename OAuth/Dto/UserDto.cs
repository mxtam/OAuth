using OAuth.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Dto
{
    public class UserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? Email { get; set; }
    }
}
