using TaskManagementAPI.Entities;

namespace TaskManagementAPI.SOLID.Liskov
{
    public class Liskov
    {
        #region Bad Example
        // Base Interface
        public interface ITaskRepository
        {
            void AddTask(TaskData task);
            List<TaskData> GetAllTasks();
        }

        // Concrete Implementation: Task Repository
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

        // Subclass Violating LSP: Read-Only Task Repository
        public class ReadOnlyTaskRepository : ITaskRepository
        {
            public void AddTask(TaskData task)
            {
                throw new NotSupportedException("Cannot add tasks in a read-only repository.");
            }

            public List<TaskData> GetAllTasks()
            {
                return new List<TaskData>(); // Returns a read-only list
            }
        }
        #endregion

        #region Good Example
        // Base Interface for Read Operations
        public interface ITaskReader
        {
            List<TaskData> GetAllTasks();
        }

        // Extended Interface for Write Operations
        public interface ITaskWriter : ITaskReader
        {
            void AddTask(TaskData task);
        }

        // Concrete Implementation: Task Repository (Full Access)
        public class TasksRepository : ITaskWriter
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

        // Concrete Implementation: Read-Only Task Repository
        public class ReadOnlyTasksRepository : ITaskReader
        {
            private readonly List<TaskData> _tasks = new();

            public List<TaskData> GetAllTasks()
            {
                return _tasks; // Return a read-only list if needed
            }
        }
        #endregion
    }
}
