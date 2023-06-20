using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<AppUser> _userManager;

        private readonly List<string> CompletionStatuses = new List<string> {
            "Open","Assigned","in Progress", "on Hold", "Done"
        };

        private readonly List<string> Priority = new List<string> {
            "High","Medium","Low"
        };

        private readonly List<string> Categories = new List<string> {
            "Electrical","Technological","Plumbing","Construction","Carpentry"
        };

        public TasksController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        #region Index 
        // 1- Order by CreatedAt: ascending: true - descending: false 
        // 2- Search Box: Title, Description, PublicUser.FullName, Location
        // 3- Filters:
        //          a) Priority: "High","Medium","Low"
        //          b) CompletionStatus : "Open","Assigned","in Progress", "on Hold", "Done"
        // GET: Tasks
        [Authorize(Policy = "RequireManagerOrWorkerRole")]
       
        public async Task<IActionResult> Index(bool showDeleted, bool ascending, string? searchString, List<string> SelectedPriority, List<string> SelectedStatuses, List<string> SelectedCategories)
        {
            List<TaskModel>? tasksList = null;

            IQueryable<TaskModel> result = _context.Tasks.Include(r => r.Worker);

            //Do not return deleted tasks if showDeleted==false

            if (!showDeleted)
            {
                result = result.Where(task => task.Deleted == false);
            }

            //If Search Box is not Empty, apply conditions on the query

            if (!String.IsNullOrEmpty(searchString))
            {
                result = result.Where(task => task.Title.Contains(searchString)
                || task.Description.Contains(searchString)
                || task.Location.Contains(searchString)
                || task.Worker.FullName.Contains(searchString));
            }

            // If Maanger selected categories, show only task of selected categories

            if (SelectedCategories != null && SelectedCategories.Count > 0)
            {
                result = result.Where(task => SelectedCategories.Contains(task.Category));
            }

            // If Manager selected completion status, show only task of selected completion statuses

            if (SelectedStatuses != null && SelectedStatuses.Count > 0)
            {
                result = result.Where(task => SelectedStatuses.Contains(task.CompletionStatus));
            }

            //  If Manager selected priority, show only task of selected Priority 

            if (SelectedPriority != null && SelectedPriority.Count > 0)
            {
                result = result.Where(task => SelectedPriority.Contains(task.Priority));
            }

            // Order By CreatedAt

            if (!ascending)
            {
                result = result.OrderByDescending(t => t.CreatedAt);
            }

            else
            {
                result = result.OrderBy(t => t.CreatedAt);
            }

            tasksList = result.ToList();

            ViewData["SelectedStatuses"] = new SelectList(CompletionStatuses);
            ViewData["SelectedCategories"] = new SelectList(Categories);
            ViewData["SelectedPriority"] = new SelectList(Priority);
            
            return View(tasksList);
        }
        #endregion


        #region Task Details
        // GET: Tasks/Details/5

        [Authorize(Policy = "RequireManagerOrWorkerRole")]
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
        #endregion


        #region GET: Tasks Create
        // GET: Tasks/Create
        //[HttpGet("[action]/{requestModel}")]

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Create(RequestModel? requestModel)
        {
            if (requestModel.Id > 0)
            {
                ViewData["Request"] = requestModel;
            }

            // Get list of workers from db
            ViewData["WorkerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Worker"), "Id" ,"FullName");


            return View();
        }
        #endregion


        #region Tasks Create
        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Location,CompletionStatus,Priority,Category,DueDate,DateCompleted,WorkerId,RequestId")] TaskModel taskModel)
        {
            taskModel.Id = 0;
            if (ModelState.IsValid)
            {
                _context.Add(taskModel);
                await _context.SaveChangesAsync();

                if (taskModel.RequestId > 0)
                {
                    var request = await _context.Requests
                                    .FindAsync(taskModel.RequestId);

                    request.ApprovalStatus = "Approved";

                    _context.Entry(request).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Requests");
                }

                return RedirectToAction(nameof(Index));
            }
            
            return View(taskModel);
        }
        #endregion


        #region GET: Tasks Edit 
        // GET: Tasks/Edit/5

        [Authorize(Policy = "RequireManagerRole")]
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
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description", taskModel.RequestId);
            ViewData["WorkerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Worker"), "Id", "FullName");
            return View(taskModel);
        }
        #endregion


        #region Edit Task
        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "RequireManagerRole")]
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
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Description", taskModel.RequestId);
            ViewData["WorkerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Worker"), "Id", "FullName");
            return View(taskModel);
        }
        #endregion


        #region UndoDelete
        // GET: Tasks/UndoDelete/5

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UndoDelete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

           var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel != null)
            {
                taskModel.Deleted = false;

                _context.Entry(taskModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Delete Task
        // GET: Tasks/Delete/5

        [Authorize(Policy = "RequireManagerRole")]
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
        #endregion


        #region Delete Confirmed
        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RequireManagerRole")]
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
        #endregion

        private bool TaskModelExists(int id)
        {
          return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
