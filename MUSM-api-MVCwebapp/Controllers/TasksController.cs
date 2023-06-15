using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Models;

namespace MUSM_api_MVCwebapp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string sortOrder)
        {
           
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var tasks = from s in _context.Tasks
                           select s;
            switch (sortOrder)
            {
                
                case "priority_desc":
                    tasks = tasks.OrderByDescending(s => s.Priority);
                    break;
                case "Date":
                    tasks = tasks.OrderBy(s => s.DueDate);
                    break;
                case "date_desc":
                    tasks = tasks.OrderByDescending(s => s.DueDate);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.CompletionStatus);
                    break;
            }


            var applicationDbContext = _context.Tasks.Include(t => t.Request).Include(t => t.Worker);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Request)
                .Include(t => t.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskModel == null)
            {
                return NotFound();
            }

            return View(taskModel);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description");
            ViewData["WorkerId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Location,CompletionStatus,Priority,Category,DueDate,DateCompleted,WorkerId")] TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description", taskModel.RequestId);
            ViewData["WorkerId"] = new SelectList(_context.Users, "Id", "FullName", taskModel.WorkerId);
            return View(taskModel);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }
            //ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description", taskModel.RequestId);
            ViewData["WorkerId"] = new SelectList(_context.Users, "Id", "FullName", taskModel.WorkerId);
            return View(taskModel);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,CompletionStatus,Priority,Category,DueDate,DateCompleted,WorkerId,RequestId,CreatedAt,Deleted")] TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskModelExists(taskModel.Id))
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
            //ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description", taskModel.RequestId);
            ViewData["Worker"] = new SelectList(_context.Users, "Id", "FullName", taskModel.WorkerId);
            return View(taskModel);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Request)
                .Include(t => t.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskModel == null)
            {
                return NotFound();
            }

            return View(taskModel);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
            }
            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel != null)
            {
                taskModel.Deleted = true;

                _context.Entry(taskModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            
           
            return RedirectToAction(nameof(Index));
        }

        private bool TaskModelExists(int id)
        {
          return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
