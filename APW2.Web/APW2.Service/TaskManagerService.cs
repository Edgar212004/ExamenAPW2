using APW2.Data.Models;
using APW2.Data.Repositorio;

namespace APW2.Services
{

    public interface ITaskManagerService
    {
        IEnumerable<TaskManager> GetAllTasks();
        TaskManager? GetTaskById(int id);
        void CreateTask(TaskManager task);
        void UpdateTask(TaskManager task);
        void DeleteTask(int id);
        IEnumerable<TaskManager> GetTasksByStatus(string status);
        IEnumerable<TaskManager> GetTasksByPriority(string priority);
        IEnumerable<TaskManager> GetArchivedTasks();
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskManagerRepository _repository;

        public TaskManagerService(ITaskManagerRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<TaskManager> GetAllTasks()
        {
            return _repository.GetAll();
        }

        public TaskManager? GetTaskById(int id)
        {
            return _repository.GetById(id);
        }

        public void CreateTask(TaskManager task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            // Business logic can be added here if necessary
            task.CreatedDate = DateTime.Now;
            _repository.Add(task);
        }

        public void UpdateTask(TaskManager task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            // Additional validation or business logic
            task.ModifiedDate = DateTime.Now;
            _repository.Update(task);
        }

        public void DeleteTask(int id)
        {
            var task = _repository.GetById(id);
            if (task == null) throw new KeyNotFoundException($"Task with ID {id} not found.");

            // Additional logic before deletion (e.g., checking permissions)
            _repository.Delete(id);
        }

        public IEnumerable<TaskManager> GetTasksByStatus(string status)
        {
            return _repository.GetByStatus(status);
        }

        public IEnumerable<TaskManager> GetTasksByPriority(string priority)
        {
            return _repository.GetByPriority(priority);
        }

        public IEnumerable<TaskManager> GetArchivedTasks()
        {
            return _repository.GetArchivedTasks();
        }
    }
}
