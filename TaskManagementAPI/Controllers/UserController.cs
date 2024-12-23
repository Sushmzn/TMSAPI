using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.User;
using TaskManagementAPI.Entities;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(TaskManagementDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                return Ok(await _context.Users.ToListAsync());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while fetching users.");
                throw;
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateUserDto>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var userPermission = await _context.UserPermissions.Where(x=>x.UserId == user.Id).Select(x=>x.PermissionId).ToListAsync();
                var permissions = await _context.Permissions.Where(x => userPermission.Contains(x.Id)).Select(x=>x.Name).ToListAsync();

                var response = new CreateUserDto()
                {
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    Permissions = permissions
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while fetching user with ID {id}.");
                throw;
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserDto user)
        {
            try
            {
                // You might want to hash the password here before saving
                var newUser = new User()
                {
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    Password = user.Password
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var permissions = await _context.Permissions.Where(x => user.Permissions.Contains(x.Name)).Select(x => x.Id).ToListAsync();

                var userPermissions = permissions.Select(permissionId => new UserPermission
                {
                    UserId = newUser.Id,
                    PermissionId = permissionId
                }).ToList();

                await _context.UserPermissions.AddRangeAsync(userPermissions);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating user.");
                throw;
            }
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;  // Hash the password here before saving, if needed

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while updating user with ID {id}.");
                throw;
            }
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while deleting user with ID {id}.");
                throw;
            }
        }
    }
}
