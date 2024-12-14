using System.Collections.Concurrent;
using APW2.Data.Models;
using APW2.Data.Repositorio;

namespace APW2.Services
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Canceled
    }

    public interface ITaskManagerService
    {
        Task<TaskManager> ExecuteTaskAsync(int taskId, CancellationToken cancellationToken);
        Task<bool> CancelTaskAsync(int taskId);

        // Métodos originales convertidos para manejar strings
        IEnumerable<TaskManager> GetTasksByStatus(string status);
        IEnumerable<TaskManager> GetTasksByPriority(string priority);
        IEnumerable<TaskManager> GetArchivedTasks();
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskManagerRepository _repository;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningTasks;

        public TaskManagerService(ITaskManagerRepository repository)
        {
            _repository = repository;
            _runningTasks = new ConcurrentDictionary<int, CancellationTokenSource>();
        }

        // Métodos de conversión de enum a string y viceversa
        private string ConvertStatusToString(TaskStatus status)
        {
            return status.ToString();
        }

        private TaskStatus ConvertStringToStatus(string status)
        {
            return Enum.TryParse(status, out TaskStatus result)
                ? result
                : TaskStatus.Pending; // Default value
        }

        public async Task<TaskManager> ExecuteTaskAsync(int taskId, CancellationToken cancellationToken)
        {
            var task = _repository.GetById(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            // Verificar si ya hay una tarea en ejecución
            if (_runningTasks.ContainsKey(taskId))
                throw new InvalidOperationException("Task is already running.");

            // Crear un CancellationTokenSource para esta ejecución
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _runningTasks[taskId] = cts;

            try
            {
                // Cambiar estado a In Progress y registrar hora de inicio
                task.Status = ConvertStatusToString(TaskStatus.InProgress);
                task.ModifiedDate = DateTime.Now;
                _repository.Update(task);

                // Simular long-running operation
                await SimulateLongRunningOperationAsync(task, cts.Token);

                // Si no fue cancelada, marcar como completada
                if (!cts.Token.IsCancellationRequested)
                {
                    task.Status = ConvertStatusToString(TaskStatus.Completed);
                    task.CompletedDate = DateTime.Now;
                    task.ModifiedDate = DateTime.Now;
                    _repository.Update(task);
                }

                return task;
            }
            catch (OperationCanceledException)
            {
                // Manejar cancelación
                task.Status = ConvertStatusToString(TaskStatus.Canceled);
                task.ModifiedDate = DateTime.Now;
                _repository.Update(task);
                throw;
            }
            finally
            {
                // Remover la tarea de las tareas en ejecución
                _runningTasks.TryRemove(taskId, out _);
            }
        }

        private async Task SimulateLongRunningOperationAsync(TaskManager task, CancellationToken cancellationToken)
        {
            try
            {
                // Esperar 5 segundos entre tareas
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                // Simular operación de 10 segundos
                using var longRunningCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, longRunningCts.Token).Token;

                await Task.Run(() =>
                {
                    // Simular trabajo intensivo
                    for (int i = 0; i < 10; i++)
                    {
                        linkedToken.ThrowIfCancellationRequested();
                        Thread.Sleep(1000); // Simular trabajo
                    }
                }, linkedToken);
            }
            catch (OperationCanceledException)
            {
                // Manejar cancelación específicamente
                throw;
            }
        }

        public async Task<bool> CancelTaskAsync(int taskId)
        {
            if (_runningTasks.TryGetValue(taskId, out var cts))
            {
                cts.Cancel();
                return true;
            }
            return false;
        }

        // Métodos para compatibilidad con la interfaz original
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