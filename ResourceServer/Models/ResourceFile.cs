namespace ResourceServer.Models
{
    public class ResourceFile
    {
        public Guid StreamId { get; set; }

        public string Name { get; set; }

        public byte[] FileStream { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public DateTimeOffset LastWriteTime { get; set; }

        public DateTimeOffset LastAccessTime { get; set; }

        public string FileType { get; set; }

        public virtual ICollection<ResourceFileConnection> ResourceFileConnections { get; set; } = new
            List<ResourceFileConnection>();
    }
}
