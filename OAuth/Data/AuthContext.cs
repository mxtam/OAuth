using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OAuth.Models;
namespace OAuth.Data
{
    public class AuthContext:DbContext
    {
        public DbSet<AuthorizationCodeChallenge> AuthCodeChallenge { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) {
            
        }

        public AuthContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Role>()
                .HasData(new Role{ Id=1, RoleName ="User" }, 
                            new Role { Id = 2, RoleName = "Admin"});
        }
    }
}
