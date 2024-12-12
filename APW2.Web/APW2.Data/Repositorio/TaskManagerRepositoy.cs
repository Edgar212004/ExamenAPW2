using APW2.Data.Models;

namespace APW2.Data.Repositorio
{

    public interface ITaskManagerRepository
    {
        IEnumerable<TaskManager> GetAll();
        TaskManager? GetById(int id);
        void Add(TaskManager task);
        void Update(TaskManager task);
        void Delete(int id);
        IEnumerable<TaskManager> GetByStatus(string status);
        IEnumerable<TaskManager> GetByPriority(string priority);
        IEnumerable<TaskManager> GetArchivedTasks();
    }

    public class TaskManagerRepository : ITaskManagerRepository
    {
        private readonly ProcessdbContext _context;

        public TaskManagerRepository(ProcessdbContext context)
        {
            _context = context;
        }

        public IEnumerable<TaskManager> GetAll()
        {
            return _context.TaskManagers.ToList();
        }

        public TaskManager? GetById(int id)
        {
            return _context.TaskManagers.Find(id);
        }

        public void Add(TaskManager task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            _context.TaskManagers.Add(task);
            _context.SaveChanges();
        }

        public void Update(TaskManager task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            _context.TaskManagers.Update(task);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var task = GetById(id);
            if (task == null) throw new KeyNotFoundException($"Task with ID {id} not found.");

            _context.TaskManagers.Remove(task);
            _context.SaveChanges();
        }

        public IEnumerable<TaskManager> GetByStatus(string status)
        {
            return _context.TaskManagers.Where(t => t.Status == status).ToList();
        }

        public IEnumerable<TaskManager> GetByPriority(string priority)
        {
            return _context.TaskManagers.Where(t => t.Priority == priority).ToList();
        }

        public IEnumerable<TaskManager> GetArchivedTasks()
        {
            return _context.TaskManagers.Where(t => t.IsArchived == true).ToList();
        }
    }
}