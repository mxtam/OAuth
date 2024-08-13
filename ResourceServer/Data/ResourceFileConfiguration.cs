using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceServer.Models;

namespace ResourceServer.Data
{
    public class ResourceFileConfiguration : IEntityTypeConfiguration<ResourceFile>
    {
        public void Configure(EntityTypeBuilder<ResourceFile> entity)
        {
            entity.ToTable("ResourceFiles",
                tb => tb.HasComment("таблиця для файлів"));

            entity.HasKey(e => e.StreamId);

            entity.Property(e => e.StreamId).HasColumnName("stream_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.CreationTime).HasColumnName("creation_time").HasColumnType("datetimeoffset");
            entity.Property(e => e.LastWriteTime).HasColumnName("last_write_time").HasColumnType("datetimeoffset");
            entity.Property(e => e.LastAccessTime).HasColumnName("last_access_time").HasColumnType("datetimeoffset");
            entity.Property(e => e.FileType)
            .HasColumnName("file_type")
            .HasComputedColumnSql("RIGHT(Name, CHARINDEX('.', REVERSE(Name)) - 1)", stored: true);
            entity.Property(e => e.FileStream).HasColumnName("file_stream").HasColumnType("varbinary(max)").IsRequired(false);
        }
    }
}
