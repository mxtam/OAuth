namespace ResourceServer.Models
{
    public class ResourceFileConnection
    {
        public int Id { get; set; }

        public Guid IdResourceFile { get; set; }

        public string? UserName { get; set; }

        public virtual ResourceFile? IdResourceFileNavigation { get; set; }
    }
}
