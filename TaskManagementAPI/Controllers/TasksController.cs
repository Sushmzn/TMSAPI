using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Constants;
using TaskManagementAPI.Data;
using TaskManagementAPI.Entities;
using TaskManagementAPI.Exceptions;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(TaskManagementDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Tasks
        [HttpGet]
        [Authorize(Policy = PermissionConstants.TaskView)]
        public async Task<ActionResult<IEnumerable<TaskData>>> GetTasks()
        {
            try
            {
                return await _context.Tasks.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while fetching tasks.");
                throw; // Global middleware will handle it
            }
        }

        // GET: api/Tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskData>> GetTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound();
                return task;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while fetching task with ID {id}.");
                throw;
            }
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskData>> CreateTask(TaskData task)
        {
            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating a task.");
                throw;
            }
        }

        // PUT: api/Tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskData task)
        {
            try
            {
                var existingTask = await _context.Tasks.FindAsync(id);
                if (existingTask == null)
                {
                    throw new UserFriendlyException("No Data Found");
                }

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.IsCompleted = task.IsCompleted;

                _context.Tasks.Update(existingTask);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e, "Concurrency error while updating task.");
                return Conflict("A concurrency error occurred.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while updating task with ID {id}.");
                throw;
            }
        }

        // DELETE: api/Tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound();

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while deleting task with ID {id}.");
                throw;
            }
        }
    }
}
