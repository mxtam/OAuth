using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class AuthorizationCodeChallenge
    {
        public int Id { get; set;  }
        [MaxLength(60)]
        public string CodeChallenge { get; set; }
        [MaxLength(10)]
        public string CodeChallengeMethod { get; set; }
        public string UserId { get; set; }
        [MaxLength(12)]
        public string UserLanguage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
