using TaskManagementAPI.Entities;

namespace TaskManagementAPI.SOLID.InterfaceSegregation
{
    public interface IBadExample
    {
        void AddTask(TaskData task);
        List<TaskData> GetAllTasks();
        TaskData GetTaskById(int id);
        void UpdateTask(TaskData task);
        void DeleteTask(int id);
        void ArchiveTask(int id); // Not all repositories need archiving
    }
    public class BadExample
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

        public void UpdateTask(TaskData task)
        {
            // Update logic
        }

        public void DeleteTask(int id)
        {
            // Delete logic
        }

        public void ArchiveTask(int id) // This is unnecessary for this repository
        {
            throw new NotImplementedException("Archiving is not supported in this repository.");
        }
    }

    //GoodExample

    // Small, specific interfaces
    public interface ITaskCreator
    {
        void AddTask(TaskData task);
    }

    public interface ITaskReader
    {
        List<TaskData> GetAllTasks();
        TaskData GetTaskById(int id);
    }

    public interface ITaskUpdater
    {
        void UpdateTask(TaskData task);
    }

    public interface ITaskDeleter
    {
        void DeleteTask(int id);
    }

    // Repository implements only the necessary interfaces
    public class TaskRepository : ITaskCreator, ITaskReader, ITaskUpdater
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

        public void UpdateTask(TaskData task)
        {
            // Update logic
        }
    }


}
