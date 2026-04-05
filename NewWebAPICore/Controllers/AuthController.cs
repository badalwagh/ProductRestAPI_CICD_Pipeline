using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewWebAPICore.DTO_s;
using NewWebAPICore.Filters;
using WebAPICore.Data;
using WebAPICore.DTO_s;
using WebAPICore.Model;
using WebAPICore.ServiceToken;

namespace WebAPICore.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ServiceFilter(typeof(AuditLogFilter))]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.user_role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(
            new APIResponseDTO_s<User>(
                    true,
                    "User registered successfully",
                    null
                ));
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new APIResponseDTO_s<string>(
                    false,
                    "Invalid username or password",
                    null
                ));
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new APIResponseDTO_s<string>(
                    true,
                    "Logged in Successfully",
                    token
                ));
        }
    }
}
