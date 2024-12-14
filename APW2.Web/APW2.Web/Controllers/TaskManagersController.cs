using Microsoft.AspNetCore.Mvc;
using APW2.Data.Models;
using APW2.Data.Repositorio;
using APW2.Services;

namespace APW2.Web.Controllers
{
    public class TaskManagersController : Controller
    {
        private readonly ITaskManagerRepository _repository;
        private readonly ITaskManagerService _taskManagerService;

        public TaskManagersController(
            ITaskManagerRepository repository,
            ITaskManagerService taskManagerService)
        {
            _repository = repository;
            _taskManagerService = taskManagerService;
        }

        // GET: TaskManagers/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: TaskManagers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskManager = _repository.GetById(id.Value);
            if (taskManager == null)
            {
                return NotFound();
            }

            return View(taskManager);
        }

        // GET: TaskManagers
        public IActionResult Index()
        {
            return View(_repository.GetAll());
        }

        // GET: TaskManagers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var taskManager = _repository.GetById(id.Value);
            if (taskManager == null)
            {
                return NotFound();
            }
            return View(taskManager);
        }

        // POST: TaskManagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskManager taskManager)
        {
            if (ModelState.IsValid)
            {
                taskManager.CreatedDate = DateTime.Now;
                taskManager.Status = "Pending";
                _repository.Add(taskManager);
                return RedirectToAction(nameof(Index));
            }
            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return View(taskManager);
        }

        // POST: TaskManagers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaskManager taskManager)
        {
            if (id != taskManager.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    taskManager.ModifiedDate = DateTime.Now;
                    _repository.Update(taskManager);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    if (_repository.GetById(taskManager.TaskId) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return View(taskManager);
        }

        // POST: TaskManagers/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/ExecuteAllTasks
        [HttpPost]
        public async Task<IActionResult> ExecuteAllTasks()
        {
            // Filtrar solo tareas pendientes
            var tasks = _repository.GetAll()
                .Where(t => t.Status == "Pending")
                .OrderBy(t => t.CreatedDate)
                .ToList();

            var results = new List<object>();

            foreach (var task in tasks)
            {
                try
                {
                    // Timeout total de 15 minutos para todas las tareas
                    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

                    var executedTask = await _taskManagerService.ExecuteTaskAsync(task.TaskId, cts.Token);

                    results.Add(new
                    {
                        TaskId = task.TaskId,
                        Status = executedTask.Status,
                        Success = true
                    });
                }
                catch (OperationCanceledException)
                {
                    results.Add(new
                    {
                        TaskId = task.TaskId,
                        Status = "Canceled",
                        Success = false
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        TaskId = task.TaskId,
                        Status = "Error",
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            // Respuesta AJAX o redireccionamiento
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(results);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/StopTask
        [HttpPost]
        public async Task<IActionResult> StopTask(int id)
        {
            try
            {
                var result = await _taskManagerService.CancelTaskAsync(id);

                if (result)
                {
                    // Opcional: actualizar el estado de la tarea en el repositorio
                    var task = _repository.GetById(id);
                    if (task != null)
                    {
                        task.Status = "Canceled";
                        task.ModifiedDate = DateTime.Now;
                        _repository.Update(task);
                    }

                    // Si es una solicitud AJAX, devolver JSON
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Task cancelled successfully" });
                    }
                }

                // Si no es una solicitud AJAX o la cancelación falló
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Si es una solicitud AJAX, devolver error como JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                // Si no es una solicitud AJAX, lanzar la excepción
                throw;
            }
        }
    }
}