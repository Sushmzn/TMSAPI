using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Entities;
using static TaskManagementAPI.SOLID.Single.TaskService;

namespace TaskManagementAPI.SOLID.Single
{
    [ApiController]
    [Route("api/[controller]")]
    public class SingleController : ControllerBase
    {
        private readonly List<TaskData> _tasks = new();

        #region Bad Example
        [HttpGet]
        public IActionResult GetAllTasks([FromBody] TaskData task)
        {
            _tasks.Add(task);
            // Directly return the task list
            return Ok(_tasks);
        }
        #endregion

        #region Good Example
        // Controller: Focuses only on request/response handling
        [ApiController]
        [Route("api/[controller]")]
        public class TaskController : ControllerBase
        {
            private readonly ITaskService _taskService;

            public TaskController(ITaskService taskService)
            {
                _taskService = taskService;
            }

            [HttpPost]
            public IActionResult AddTask([FromBody] TaskData task)
            {
                try
                {
                    _taskService.CreateTask(task);
                    return Ok("Task added successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpGet]
            public IActionResult GetAllTasks()
            {
                var tasks = _taskService.GetTasks();
                return Ok(tasks);
            }
        }

        #endregion
    }

}
