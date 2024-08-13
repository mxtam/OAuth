namespace ResourceServer.Data
{
    public class ResourceFileService
    {
        public static async Task<byte[]> FileToByteArrayAsync(IFormFile file)
        {
            using (var fileStream =  file.OpenReadStream())
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string RenameFileName(string fileName, Guid guid)
        {
            var splitedFileName = fileName.Split(".");
            var newFileName = splitedFileName.First() + $"-{guid}." + splitedFileName.Last();

            return newFileName;
        }

        public static string ReturnFileName(string fileName)
        {
            var splitedFileName = fileName.Split("-");

            var splitedExtension = splitedFileName.Last().Split('.');

            var newFileName = splitedFileName.First() + "." + splitedExtension.Last();

            return newFileName; 
        }

        public static string FileStartsWith(string filename)
        { 
            var splitedFileName = filename.Split(".");
            var startsWith = splitedFileName.First();

            return startsWith;
        }
    }
}
