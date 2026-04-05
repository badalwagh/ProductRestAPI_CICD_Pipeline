using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewWebAPICore.DTO_s;
using NewWebAPICore.Filters;
using System.Security.Claims;
using WebAPICore.Data;
using WebAPICore.Model;

namespace NewWebAPICore.Controllers
{
    [Route("api/userdetails")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(AuditLogFilter))]
    public class UserDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserDetailsController(AppDbContext context)
        {
              _context = context;  
        }
        [HttpGet]
        public async Task<IActionResult> GetUserDetails()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized(new APIResponseDTO_s<string>(
                    false, "User is not authenticated", null));
            }

            var user = await _context.Users.Where(p => p.Id == userId).SingleOrDefaultAsync();

            return Ok(new APIResponseDTO_s<User>(
                true, "User detail fetched successfully", new User {Id = user.Id,Username = user.Username,Role = user.Role }));
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out userId);
        }
    }
}
