using APW2.Data.Models;
using System.Collections.Generic;

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

    public class TaskManagerRepository : RepositoryBase<TaskManager>, ITaskManagerRepository
    {
        public TaskManagerRepository(ProcessdbContext context) : base(context)
        {
        }

        public IEnumerable<TaskManager> GetByStatus(string status)
        {
            return Find(t => t.Status == status);
        }

        public IEnumerable<TaskManager> GetByPriority(string priority)
        {
            return Find(t => t.Priority == priority);
        }

        public IEnumerable<TaskManager> GetArchivedTasks()
        {
            return Find(t => t.IsArchived == true);
        }
    }
}