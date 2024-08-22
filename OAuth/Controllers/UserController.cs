using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuth.Data;
using OAuth.Models;

namespace OAuth.Controllers
{
    public class UserController : Controller
    {
        private readonly AuthContext _context;

        public UserController(AuthContext context) 
        { 
            _context = context;
        }

        [Authorize]
        public async Task<ActionResult<List<AuthUser>>> GetAllUsers()
        {
            return await _context.AuthUsers.ToListAsync();      
        }
    }
}
