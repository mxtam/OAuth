namespace ResourceServer.Dto
{
    public class GetLastFileDateDto
    {
        public DateTimeOffset? DateTimeCreation { get; set; }
        public string? UserName { get; set; }
    }
}
