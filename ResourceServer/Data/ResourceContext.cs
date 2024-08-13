using Microsoft.EntityFrameworkCore;
using ResourceServer.Models;

namespace ResourceServer.Data
{
    public class ResourceContext : DbContext
    {
        public ResourceContext(DbContextOptions<ResourceContext> options) : base(options) 
        { 
        }

        public virtual DbSet<ResourceFile> ResourceFiles { get; set; }
        public virtual DbSet<ResourceFileConnection> ResourceFileConnections { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ResourceFileConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceFileConnectionConfiguration());

        }
    }
}
