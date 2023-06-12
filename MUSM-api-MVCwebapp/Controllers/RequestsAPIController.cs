using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;

namespace MUSM_api_MVCwebapp.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]

    public class RequestsAPIController:ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public RequestsAPIController(ApplicationDbContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }

        [HttpGet("[action]/{withDeleted}")]
        [Authorize(Policy = "RequireManagerOrPublicUserRole")]
        public async Task<ActionResult> GetRequests([FromRoute] bool withDeleted)
        {
            List<RequestModel>? requestsList = null;

            if (withDeleted == true){

                requestsList = await _db.Requests
                    .ToListAsync();
            }
            else
            {
                requestsList = await _db.Requests
                    .Where(request => request.Deleted == false)
                    .ToListAsync();
            }

            if (requestsList == null || requestsList.Count() <= 0) return NotFound(new JsonResult("No Requests existed."));

            return Ok(requestsList);
        }

        //URL: https://localhost:7058/api/requestsapi/CreateRequest
        [HttpPost("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> CreateRequest([FromBody] RequestDto data)
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
