using TaskManagementAPI.Entities;

namespace TaskManagementAPI.SOLID.OpenClosed
{
    public class OpenClosed
    {
        #region GoodExample
        // Base class that handles task operations
        public class TasksManager
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

            public TaskData GetTaskById(int id)
            {
                return _tasks.FirstOrDefault(t => t.Id == id);
            }

            public void CompleteTask(int id)  // Modifies existing functionality, violates OCP
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    task.IsCompleted = true;
                }
            }
        }

        #endregion
        #region Good Example

        // Base interface for common task operations
        public interface ITaskOperations
        {
            List<TaskData> GetAllTasks();
            TaskData GetTaskById(int id);
        }

        // Interface for task creation
        public interface ITaskCreator : ITaskOperations
        {
            void AddTask(TaskData task);
        }

        // Interface for task completion (new functionality)
        public interface ITaskCompleter : ITaskOperations
        {
            void CompleteTask(int id);
        }

        // Implementation of ITaskCreator
        public class TaskManager : ITaskCreator
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

            public TaskData GetTaskById(int id)
            {
                return _tasks.FirstOrDefault(t => t.Id == id);
            }
        }

        // New implementation for task completion
        public class TaskCompleter : ITaskCompleter
        {
            private readonly List<TaskData> _tasks = new();

            public TaskCompleter(List<TaskData> tasks)
            {
                _tasks = tasks;
            }

            public List<TaskData> GetAllTasks()
            {
                return _tasks;
            }

            public TaskData GetTaskById(int id)
            {
                return _tasks.FirstOrDefault(t => t.Id == id);
            }

            public void CompleteTask(int id)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    task.IsCompleted = true;
                }
            }
        }
        #endregion
    }
}
