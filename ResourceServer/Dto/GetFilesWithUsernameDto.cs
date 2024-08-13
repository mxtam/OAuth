namespace ResourceServer.Dto
{
    public class GetFilesWithUsernameDto
    {
        public Guid StreamId { get; set; }

        public string? FileName { get; set; }

        public string? UserName { get; set;  }
    }
}
