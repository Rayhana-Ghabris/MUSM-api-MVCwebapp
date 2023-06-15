using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;

namespace MUSM_api_MVCwebapp.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // TODO paging + filtering 
        // GET: Requests

        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "CreatedAt" ? "CreatedAt_desc" : "Date";
            var requests = from s in _context.Requests
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                requests = requests.Where(s => s.Description.Contains(searchString)
                                       || s.Title.Contains(searchString)
                                       || s.PublicUser.FullName.Contains(searchString)
                                       || s.Location.Contains(searchString)
                                       && (s.ApprovalStatus.StartsWith(searchString) 
                                           || s.Category.StartsWith(searchString) ));
            }

            switch (sortOrder)
            {

                case "Date":
                    requests = requests.OrderBy(s => s.CreatedAt);
                    break;

                case "date_desc":
                    requests = requests.OrderByDescending(s => s.CreatedAt);
                    break;

            }

            return View(requests.ToList());
        }
        /*
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Requests.Include(r => r.PublicUser);
            return View(await applicationDbContext.ToListAsync());
        }*/

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests
                .Include(r => r.PublicUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestModel == null)
            {
                return NotFound();
            }

            return View(requestModel);
        }




        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests.FindAsync(id);
            if (requestModel == null)
            {
                return NotFound();
            }
            // Get list of workers from db
            ViewData["Workers"] = await (
                    from user in _context.Users
                    join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                    join role in _context.Roles
                    on userRole.RoleId equals role.Id
                    where role.Name == "Worker"
                    select new AppUserDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,

                    }).ToListAsync();

            return View(requestModel);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ApprovalStatus,Location,Category")] RequestModel requestModel)
        {
            if (id != requestModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Tasks.Add(requestModel.Task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestModelExists(requestModel.Id))
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
            ViewData["Workers"] = await (
                    from user in _context.Users
                    join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                    join role in _context.Roles
                    on userRole.RoleId equals role.Id
                    where role.Name == "Worker"
                    select new AppUserDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,

                    }).ToListAsync();

            return View(requestModel);
        }


        // POST: Requests/Rejected/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, [Bind("Id,Title,Description,ApprovalStatus,Location,Category")] RequestModel requestModel)
        {
            if (id != requestModel.Id)
            {
                return NotFound();
            }

            requestModel = await _context.Requests.FindAsync(id);

            if (requestModel != null)
            {

                requestModel.ApprovalStatus = "Reject";

                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

        
        // POST: Requests/Undo Evaluation/5
        [HttpPost, ActionName("UndoEvaluation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoEvaluation(int id, [Bind("Id,Title,Description,ApprovalStatus,Location,Category")] RequestModel requestModel)
        {
            if (id != requestModel.Id)
            {
                return NotFound();
            }

            requestModel = await _context.Requests.FindAsync(id);

            if (requestModel != null)
            {
               if (requestModel.ApprovalStatus.Equals("Aprove") || requestModel.ApprovalStatus.Equals("Reject") || requestModel.ApprovalStatus.Equals("Delete") )
                {
                    requestModel.ApprovalStatus  = "Under Evaluation";
                  
                }

             
                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
       


        #region
        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests
                .Include(r => r.PublicUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestModel == null)
            {
                return NotFound();
            }

            return View(requestModel);
        }
        #endregion


        #region
        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Requests == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Requests'  is null.");
            }
            var requestModel = await _context.Requests.FindAsync(id);
            if (requestModel != null)
            {

                requestModel.Deleted = true;

                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        private bool RequestModelExists(int id)
        {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
    #endregion



    //AssignToWorker
   /* public async Task<IActionResult> AssignToWorker(int id)
    {

    }*/
}