using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using System.Security.Claims;

namespace MUSM_api_MVCwebapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public RequestsAPIController(ApplicationDbContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }
        
        [HttpGet("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> GetRequests([FromQuery] string? categorySearch, [FromQuery] string? locationSearch, [FromQuery] string? keyword)
        {
            List<RequestModel>? requestsList = null;

            var result = _db.Requests
              .Where(request => request.Deleted == false);

            if (!String.IsNullOrEmpty(categorySearch))
            {
                result = result.Where(request => request.Category.Equals(categorySearch));
            }

            if (!String.IsNullOrEmpty(locationSearch))
            {
                result = result.Where(request => request.Location.Equals(locationSearch));
            }

            if (!String.IsNullOrEmpty(keyword))
            {
                result = result.Where(request => request.Description.Contains(keyword) || request.Title.Contains(keyword));
            }

            requestsList = result.ToList();

            if (requestsList == null || requestsList.Count() <= 0) return NotFound(new JsonResult("No Requests existed."));

            return Ok(requestsList);
        }

        //GetRequest([route]id)*
        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> GetMyRequests([FromRoute] string id)
        {

            var requestsList = _db.Requests
                .Where(request => request.PublicUserId.Equals(id));

            if (requestsList == null || requestsList.Count() <= 0) return NotFound(new JsonResult("No Requests existed."));

            return Ok(requestsList);

        }

        //GetRequestCompletionStatus([route]idofrequest)
        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public ActionResult GetRequestCompletionStatus([FromRoute] int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var task = _db.Tasks
                .Where(task => task.RequestId == id)
                .FirstOrDefault();

            if (task == null) return NotFound();

            return Ok(task.CompletionStatus);

        }

        //change createRequest-->PostRequest*
        //UpdateRequest([route]idofrequest, [body]updatedrequest)
        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> UpdateRequest([FromRoute] int id, [FromBody] RequestDto data)
        {

            if (id <= 0)
            {
                return NotFound();
            }

            var request =  await _db.Requests
               .FindAsync(id);

            if (!request.PublicUserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value) 
                || request == null)
            {
                return NotFound();
            }

            // If the request was found
            request.Title = data.Title;
            request.Description = data.Description;
            request.Photo = data.Photo;
            request.Location = data.Location;
            request.Category = data.Category;
         
            _db.Entry(request).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok("Successfully updated");
        }

        //DeleteRequest([route]idofrequest) deleted=true
        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> DeleteRequest([FromRoute] int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var request = await _db.Requests
              .FindAsync(id);           

            if (!request.PublicUserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                 || request == null)
            {
                return NotFound();
            }
                request.Deleted = true;

            _db.Entry(request).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok("Successfully deleted");

        }

        //URL: https://localhost:7058/api/requestsapi/PostRequest
        [HttpPost("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> PostRequest([FromBody] RequestDto data)
        {
            RequestModel request = _mapper.Map<RequestModel>(data);

            request.PublicUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var createRequest = await _db.Requests.AddAsync(request);

            if (createRequest.State.ToString() != "Added")
                return BadRequest(new JsonResult("Failed to create new Request"));

            await _db.SaveChangesAsync();

            return Created("", createRequest.Entity);

        }
    }
}
