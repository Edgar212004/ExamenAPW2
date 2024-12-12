using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APW2.Data.Models;

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Name,Description,Status,Priority,AssignedTo,CreatedDate,ModifiedDate,DueDate,CompletedDate,Category,Notes,IsArchived")] TaskManager taskManager)
        {
            if (ModelState.IsValid)
            {
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        private bool TaskManagerExists(int id)
        {
            return _context.TaskManagers.Any(e => e.TaskId == id);
        }
    }
}
