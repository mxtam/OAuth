namespace OAuth.Models
{
    public class AuthorizationCodeChallenge
    {
        public int Id { get; set;  }
        public string CodeChallenge { get; set; }
        public string CodeChallengeMethod { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
