using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OAuth.Models;
namespace OAuth.Data
{
    public class AuthContext:DbContext
    {
        public DbSet<AuthorizationCodeChallenge> AuthCodeChallenge { get; set; }
        
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) {
            
        }

        public AuthContext()
        {
        }
    }
}
