using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuth.Data;
using OAuth.Dto;
using OAuth.Models;

namespace OAuth.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly AuthContext _context;

        public UserController(AuthContext context)
        {
            _context = context;
        }

        [HttpGet("getAllUser")]
        public async Task<ActionResult<List<UserDto>>> GetAllAuthUsers()
        {
            var originHeader = Request.Headers["Origin"].ToString();

            if (originHeader == "http://localhost:7002")
            {
                return Ok(await _context.AuthUsers.Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Patronymic = u.Patronymic,
                    Email = u.Email,
                }).ToListAsync());
            }
            else 
            { 
                return Unauthorized();
            }
        }
    }
}
