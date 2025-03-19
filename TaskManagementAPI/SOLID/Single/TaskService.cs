using TaskManagementAPI.Entities;

namespace TaskManagementAPI.SOLID.Single
{
    public class TaskService
    {
        public interface ITaskRepository
        {
            void AddTask(TaskData task);
            List<TaskData> GetAllTasks();
        }

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

        // Service: Contains business logic
        public interface ITaskService
        {
            void CreateTask(TaskData task);
            List<TaskData> GetTasks();
        }

        public class TaskServices : ITaskService
        {
            private readonly ITaskRepository _taskRepository;

            public TaskServices(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            public void CreateTask(TaskData task)
            {
                if (string.IsNullOrEmpty(task.Title))
                {
                    throw new ArgumentException("Task title is required.");
                }

                _taskRepository.AddTask(task);
            }

            public List<TaskData> GetTasks()
            {
                return _taskRepository.GetAllTasks();
            }
        }
    }
}
