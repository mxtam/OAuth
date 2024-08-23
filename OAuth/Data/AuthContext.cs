using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OAuth.Models;
namespace OAuth.Data
{
    public class AuthContext : DbContext
    {
        public DbSet<AuthorizationCodeChallenge> AuthCodeChallenges { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthUser>().ToTable("AuthUsers", "Auth");

            modelBuilder.Entity<AuthorizationCodeChallenge>().ToTable("AuthCodeChallenges", "Auth");

            modelBuilder.Entity<Role>().ToTable("Roles", "Auth");

            //Заповнюємо початковими даними таблицю Role
            modelBuilder.Entity<Role>()
                .HasData(new Role { Id = 1, RoleName = "User" },
                            new Role { Id = 2, RoleName = "Admin" });

            //Заповнюємо початковими даними таблицю AuthUser
            modelBuilder.Entity<AuthUser>()
                .HasData(new AuthUser
                {
                    Id = 1,
                    Email = "email@mail.com",
                    FirstName = "Jack",
                    LastName = "Daniels",
                    Patronymic = "Morgan",
                    //Password:password1
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1"),
                    //Role:User
                    RoleId = 1,
                    Language = "uk-UA"
                },

                new AuthUser
                {
                    Id = 2,
                    Email = "admin_mail@mail.com",
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Patronymic = "Adminovych",
                    //Password:Admin@1
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1"),
                    //Role:Admin
                    RoleId = 2,
                    Language = "en-US"
                })
;
        }
    }
}
