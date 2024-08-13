using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceServer.Models;

namespace ResourceServer.Data
{
    public class ResourceFileConnectionConfiguration : IEntityTypeConfiguration<ResourceFileConnection>
    {
        public void Configure(EntityTypeBuilder<ResourceFileConnection> entity)
        {
            entity.ToTable("ResourceFileConnection",
                tb => tb.HasComment("таблиця для зв'язку файлів"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdResourceFile).HasColumnName("ID_ResourceFile");

            entity.HasOne(d => d.IdResourceFileNavigation).WithMany(p => p.ResourceFileConnections)
                  .HasForeignKey(d => d.IdResourceFile)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Resource_FileConnections_ResourceFile");
        }
    }
}
