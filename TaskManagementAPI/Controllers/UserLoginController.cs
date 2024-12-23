using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLoginController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<UserLoginController> _logger;
        private readonly JwtTokenService _jwtTokenService;

        public UserLoginController(TaskManagementDbContext context, ILogger<UserLoginController> logger, JwtTokenService jwtTokenService)
        {
            _context = context;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        // POST: api/UserLogin
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            try
            {
                // Find the user by username or email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName || u.Email == loginRequest.Email);
                if (user == null)
                {
                    return Unauthorized("Invalid credentials.");
                }
                var userPermissions = await _context.UserPermissions.Where(x => x.UserId == user.Id).Select(x => x.PermissionId).ToListAsync();
                var permissionNames = await _context.Permissions.Where(x => userPermissions.Contains(x.Id)).Select(x => x.Name).ToListAsync();

                // Verify password (in a real scenario, you should hash the password and compare the hashed values)
                if (user.Password != loginRequest.Password) // In production, hash and compare password hashes.
                {
                    return Unauthorized("Invalid credentials.");
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(user.UserName, permissionNames);

                return Ok(new { Token = token });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during login.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Request DTO for login
        public class UserLoginRequest
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
