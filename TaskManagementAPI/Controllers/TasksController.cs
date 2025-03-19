using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Constants;
using TaskManagementAPI.Data;
using TaskManagementAPI.Entities;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<TasksController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TasksController(TaskManagementDbContext context, ILogger<TasksController> logger, IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                if (task == null)
                { return NotFound(); }
                return task;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while fetching task with ID {id}.");
                throw;
            }
        }

        //UnitOfWork used in Repositories
        //Transactions used in DbContext

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

        [HttpPost]
        public async Task<ActionResult<TaskData>> CreateTaskWithTransaction(TaskData task)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Tasks.Add(task);
                    await _context.SaveChangesAsync();
                    await transaction.CreateSavepointAsync("Task1 saved");

                    var task2 = new TaskData();
                    _context.Tasks.Add(task2);
                    await _context.SaveChangesAsync();

                    // Commit the transaction if everything goes well
                    //it auto rollback if issue occurs
                    await transaction.CommitAsync();
                    return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
                }
                catch (Exception e)
                {
                    // Rollback the transaction if an error occurs
                    await transaction.RollbackAsync();

                    //rollback to saved point
                    await transaction.RollbackToSavepointAsync("BeforeMoreBlogs");

                    _logger.LogError(e, "Error occurred while creating a task.");
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskData>> CreateTaskWithUOW(TaskData task)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();  // Begin the transaction

                _context.Tasks.Add(task);
                await _unitOfWork.SaveAsync();  // Save changes

                await _unitOfWork.CommitAsync();  // Commit the transaction if everything goes well
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();  // Rollback the transaction if an error occurs

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
                if (task == null)
                {
                    return NotFound();
                }

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
