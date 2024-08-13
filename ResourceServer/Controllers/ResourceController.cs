using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceServer.Data;
using ResourceServer.Dto;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    [ApiController]
    [Route("resources")]
    public class ResourceController:Controller
    {
        private readonly ResourceContext _context;

        public ResourceController(ResourceContext context)
        { 
            _context = context;
        }    

        [Authorize]
        [HttpGet("helloUser")]
        public IActionResult GetUserName()
        {
            //Отримуємо користувача
            var user = HttpContext.User?.Identity?.Name;

            //Отримуємо мову користувача з клеймів токена 
            var userLang = HttpContext.User?.Claims?.FirstOrDefault(x=>x.Type== "UserLang")?.Value;


            if (userLang == "uk-UA")
            {
                return Ok($"Привіт, {user}!");
            }

            return Ok($"Hello, {user}!");
        }

        [Authorize]
        [HttpPost("uploadFiles")]
        public async Task<ActionResult<string>> PostResourceFiles(IFormFile file)
        {
            const double MAX_FILE_SIZE = 20 * 1024 * 1024;

            var userName = HttpContext.User?.Identity?.Name;

            var guid = Guid.NewGuid();

            var newFileName = ResourceFileService.RenameFileName(file.FileName, guid);

            if (file == null)
            {
                return BadRequest("Файл не прикріплено");
            }

            if (!file.FileName.Contains(".xml"))
            {
                return BadRequest("Можливо завантажити лише XML файли");
            }

            if (file.Length == 0)
            {
                return BadRequest("Неможливо прикріпити пустий файл");
            }

            if (file.Length >= MAX_FILE_SIZE)
            {
                return BadRequest("Файл не може бути більше 20 мб");
            }

            using (var tranasction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var resourceFile = new ResourceFile
                    {
                        StreamId = guid,
                        Name = newFileName,
                        FileType = file.FileName,
                        FileStream = await ResourceFileService.FileToByteArrayAsync(file),
                        CreationTime = DateTime.Now,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = DateTime.Now
                    };

                    var fileConnection = new ResourceFileConnection
                    {
                        IdResourceFile = guid,
                        Username = userName
                    };

                    await _context.ResourceFiles.AddAsync(resourceFile);
                    await _context.ResourceFileConnections.AddAsync(fileConnection);
                    await _context.SaveChangesAsync();

                    await tranasction.CommitAsync();
                }
                catch (Exception ex)
                { 
                    await tranasction.RollbackAsync();
                    return BadRequest(ex);
                }
            }
                
            return Ok("Файл збережено успішно");
        }

        [Authorize]
        [HttpGet("myFiles/list")]
        public async Task<ActionResult<List<GetFilesDto>>> GetMyFiles()
        {
            var userName = HttpContext.User?.Identity?.Name;

            var usersFiles = await _context.ResourceFileConnections.Where(f=>f.Username==userName)
                .Include(f=>f.IdResourceFileNavigation)
                .Select(f => new GetFilesDto
                {
                    StreamId = f.IdResourceFile,
                    Name = ResourceFileService.ReturnFileName(f.IdResourceFileNavigation.Name),
                }).ToListAsync();

            return Ok(usersFiles);
        }

        [HttpGet("files/list")]
        public async Task<ActionResult<List<GetFilesWithUsernameDto>>> GetFiles()
        {
            var files = await _context.ResourceFileConnections
                .Include(c=>c.IdResourceFileNavigation)
                .Select(c => new GetFilesWithUsernameDto 
                { 
                    StreamId =c.IdResourceFile,
                    FileName = ResourceFileService.ReturnFileName(c.IdResourceFileNavigation.Name),
                    UserName = c.Username
                }).ToListAsync();

            return Ok(files);
        }

        [HttpGet("files/{fileName}")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            var file = await _context.ResourceFiles.Where(f => 
            f.Name.StartsWith(ResourceFileService.FileStartsWith(fileName)))
                .OrderByDescending(f => f.CreationTime).FirstOrDefaultAsync();

            if (file == null)
            {
                return BadRequest("Файл не знайдено");
            }

            return File(file.FileStream, "application/octet-stream", ResourceFileService.ReturnFileName(file.Name));
        }
    }
}
