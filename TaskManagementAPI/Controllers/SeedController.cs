using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Constants;
using TaskManagementAPI.Data;
using TaskManagementAPI.Entities;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(TaskManagementDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task SeedAsync()
        {
            try
            {
                var permissions = new List<Permission>()
                {
                    new Permission { Name = PermissionConstants.TaskView, Key = "Task" },
                    new Permission { Name = PermissionConstants.TaskCreate, Key = "Task" },
                    new Permission { Name = PermissionConstants.TaskEdit , Key = "Task"},
                    new Permission { Name = PermissionConstants.TaskDelete , Key = "Task"},


                    new Permission { Name =PermissionConstants.UserView, Key = "User" },
                    new Permission { Name = PermissionConstants.UserCreate, Key = "User" },
                    new Permission { Name = PermissionConstants.UserEdit , Key = "User"},
                    new Permission { Name = PermissionConstants.UserDelete , Key = "User"}
                };
                await _context.Permissions.AddRangeAsync(permissions);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while seeding.");
                throw;
            }
        }
    }
}
