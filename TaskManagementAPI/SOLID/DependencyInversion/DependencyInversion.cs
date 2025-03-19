using TaskManagementAPI.Data;
using TaskManagementAPI.Entities;

namespace TaskManagementAPI.SOLID.DependencyInversion
{
    public class DependencyInversion
    {
        //Bad Example
        public class TaskManager
        {
            private readonly TaskManagementDbContext taskManagementDbContext;

            public TaskManager(TaskManagementDbContext taskManagementDbContext)
            {
                this.taskManagementDbContext = taskManagementDbContext;
            }

            public void AddTask(TaskData task)
            {
                taskManagementDbContext.Tasks.Add(task);  // Direct dependency on low-level module
            }

            public List<TaskData> GetAllTasks()
            {
                return taskManagementDbContext.Tasks.ToList();
            }
        }
        // Abstraction interface for TaskRepository

        #region Good Example
        public interface ITaskRepository
        {
            void AddTask(TaskData task);
            List<TaskData> GetAllTasks();
        }

        // Concrete implementation of ITaskRepository
        public class TaskRepository : ITaskRepository
        {
            private readonly List<TaskData> _tasks = new();

            public void AddTask(TaskData task)
            {
                _tasks.Add(task);
            }

            public List<TaskData> GetAllTasks()
            {
                return _tasks;
            }
        }

        // High-level module (TaskManager) depends on abstraction
        public class TasksManager
        {
            private readonly ITaskRepository _taskRepository;

            public TasksManager(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;  // Dependency injection of abstraction
            }

            public void AddTask(TaskData task)
            {
                _taskRepository.AddTask(task);  // High-level module depends on abstraction
            }

            public List<TaskData> GetAllTasks()
            {
                return _taskRepository.GetAllTasks();
            }
        }
        #endregion
    }
}