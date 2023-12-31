﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using MUSM_api_MVCwebapp.Services;

namespace MUSM_api_MVCwebapp.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        private readonly VotesService _votesService;

        private readonly MediaService _mediaService;

        private readonly List<string> ApprovalStatuses = new List<string> {
            "Approved","Rejected","Under Evaluation"
        };

        private readonly List<string> Categories = new List<string> {
            "Electrical","Technological","Plumbing","Construction","Carpentry"
        };

        public RequestsController(ApplicationDbContext context, UserManager<AppUser> userManager, VotesService votesService, MediaService mediaService)
        {
            _context = context;
            _userManager = userManager;
            _votesService = votesService;
            _mediaService = mediaService;
        }



        #region Index
        // TODO paging + filtering 
        // 1- Order by CreatedAt: ascending: true - descending: false 
        // 2- Search Box: Title, Description, PublicUser.FullName, Location
        // 3- Filters:
        //          a) SelectedCategories: Electical, Technological, Plumbing , Constraction, Carpentry
        //          b) ApprovalStatus : Approved, Rejected, Under Evaluation
        // GET: Requests

        [Authorize(Policy = "RequireManagerRole")]
        public ActionResult Index(bool showDeleted, bool ascending, string? searchString, List<string> SelectedCategories, List<string> SelectedStatuses)
        {
            
            List<RequestModel>? requestsList = null;

            IQueryable<RequestModel> result = _context.Requests.Include( r => r.PublicUser);

            //Do not return deleted requests if showDeleted==false

            if (!showDeleted)
            {
                result = result.Where(request => request.Deleted == false);
            }
            
            //If Search Box is not Empty, apply conditions on the query

            if (!String.IsNullOrEmpty(searchString))
            {
                result = result.Where(request => request.Title.Contains(searchString)
                || request.Description.Contains(searchString)
                || request.Location.Contains(searchString)
                || request.PublicUser.FullName.Contains(searchString));
            }

            // If Maanger selected categories, show only request of selected categories

            if (SelectedCategories!=null && SelectedCategories.Count>0)
            {
                result = result.Where(request => SelectedCategories.Contains(request.Category));
            }

            // If Maanger selected approval status, show only request of selected approval statuses

            if (SelectedStatuses != null && SelectedStatuses.Count > 0)
            {
                result = result.Where(request => SelectedStatuses.Contains(request.ApprovalStatus));
            }

            // Order By CreatedAt

            if (!ascending)
            {
                result = result.OrderByDescending(r => r.CreatedAt);
            }

            else
            {
                result = result.OrderBy(r => r.CreatedAt);
            }

            requestsList = result.ToList();

            foreach (var item in requestsList)
            {
                item.VotesCount = _votesService.CountVotesByRequestId(item.Id);
            }

            ViewData["SelectedStatuses"] = new SelectList(ApprovalStatuses);
            ViewData["SelectedCategories"] = Categories;
           
            return View(requestsList);
        }
        #endregion


        #region Request Details
        // GET: Requests/Details/5
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests
                .Include(r => r.PublicUser)
                .Include(p => p.Photo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestModel == null)
            {
                return NotFound();
            }
            requestModel.VotesCount = _votesService.CountVotesByRequestId(id);
            
            return View(requestModel);
        }
        #endregion


        #region Approve Request
        // GET: Requests/Approve/5
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Approve(int? id)
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

            return RedirectToAction("Create", "Tasks", requestModel);
        }
        #endregion

        #region Reject Request

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Reject(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests.FindAsync(id);

            if (requestModel != null)
            {

                requestModel.ApprovalStatus = "Rejected";

                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Undo Evaluation

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UndoEvaluation(int? id)
        {
            if (id <= 0 )
            {
                return NotFound();
            }

            var requestModel = await _context.Requests.FindAsync(id);

            if (requestModel != null)
            {
                requestModel.ApprovalStatus  = "Under Evaluation";

                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            var task = await _context.Tasks.Where(t => t.RequestId == id).FirstAsync();

            if (task != null)
            {
                task.RequestId = null;
                task.Deleted = true;
                task.WorkerId = null;

                _context.Entry(task).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
        #endregion

        #region UndoDelete
        // GET: Requests/UndoDelete/5

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UndoDelete(int? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var requestModel = await _context.Requests.FindAsync(id);
            if (requestModel != null)
            {
                requestModel.Deleted = false;

                _context.Entry(requestModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        // GET: Requests/Delete/5
        [Authorize(Policy = "RequireManagerRole")]
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

        #region Delete Confirmation

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]

        [Authorize(Policy = "RequireManagerRole")]
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
        #endregion

        #region RequestModelExists
        private bool RequestModelExists(int id)
        {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion

        public async Task<IActionResult> ClearFilter()
        {
            return RedirectToAction(nameof(Index));
        }



    }




}