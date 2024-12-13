using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APW2.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APW2.Web.Controllers
{
    public class TaskManagersController : Controller
    {
        private readonly ProcessdbContext _context;

        public TaskManagersController(ProcessdbContext context)
        {
            _context = context;
        }

        // GET: TaskManagers
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaskManagers.ToListAsync());
        }

        // GET: TaskManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskManager = await _context.TaskManagers
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskManager == null)
            {
                return NotFound();
            }

            return View(taskManager);
        }

        // GET: TaskManagers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskManagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Name,Description,Status,Priority,AssignedTo,CreatedDate,ModifiedDate,DueDate,CompletedDate,Category,Notes,IsArchived")] TaskManager taskManager)
        {
            if (ModelState.IsValid)
            {
                taskManager.CreatedDate = DateTime.Now;
                taskManager.Status = "Pending"; // Set default status
                _context.Add(taskManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskManager);
        }

        // GET: TaskManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskManager = await _context.TaskManagers.FindAsync(id);
            if (taskManager == null)
            {
                return NotFound();
            }
            return View(taskManager);
        }

        // POST: TaskManagers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,Name,Description,Status,Priority,AssignedTo,CreatedDate,ModifiedDate,DueDate,CompletedDate,Category,Notes,IsArchived")] TaskManager taskManager)
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
                    _context.Update(taskManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskManagerExists(taskManager.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taskManager);
        }

        // GET: TaskManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskManager = await _context.TaskManagers
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskManager == null)
            {
                return NotFound();
            }

            return View(taskManager);
        }

        // POST: TaskManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskManager = await _context.TaskManagers.FindAsync(id);
            if (taskManager != null)
            {
                _context.TaskManagers.Remove(taskManager);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/ExecuteAllTasks
        [HttpPost]
        public async Task<IActionResult> ExecuteAllTasks()
        {
            var tasks = await _context.TaskManagers
                .Where(t => t.Status != "Completed" && t.Status != "Stopped")
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Status = "In Progress";
                task.ModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/StopTask/5
        [HttpPost]
        public async Task<IActionResult> StopTask(int id)
        {
            var task = await _context.TaskManagers.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Status = "Stopped";
            task.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/CompleteTask/5
        [HttpPost]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.TaskManagers.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Status = "Completed";
            task.CompletedDate = DateTime.Now;
            task.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: TaskManagers/NewLoad
        [HttpPost]
        public IActionResult NewLoad()
        {
            // Check if there are any tasks
            var hasTasks = _context.TaskManagers.Any();

            if (hasTasks)
            {
                return BadRequest("Cannot create new load when tasks exist.");
            }

            // Logic for creating a new batch of tasks could go here
            return RedirectToAction(nameof(Create));
        }

        private bool TaskManagerExists(int id)
        {
            return _context.TaskManagers.Any(e => e.TaskId == id);
        }
    }
}