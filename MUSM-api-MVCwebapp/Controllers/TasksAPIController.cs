using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksAPIController:ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public TasksAPIController(ApplicationDbContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult> GetTasks()
        {       
            
            var tasks = await _db.Tasks.ToListAsync();

            if (tasks == null || tasks.Count() <= 0) return NotFound(new JsonResult("No Tasks existed."));


            return Ok(tasks);
        }


        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> GetTasks([FromRoute] string id)
        {

            var tasks = await _db.Tasks.ToListAsync();

            if (tasks == null || tasks.Count() <= 0) return NotFound(new JsonResult("No Tasks existed."));

            return Ok(tasks);


            
            
        }


        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto data, [FromRoute] string id )
        {
           
            TaskModel task = _mapper.Map<TaskModel>(data);

            task.WorkerId = id;

            var createTask = await _db.Tasks.AddAsync(task);
            
           
            if (createTask.State.ToString() != "Added")
                return BadRequest(new JsonResult("Failed to create new Task"));

            await _db.SaveChangesAsync();

            return Created("", createTask.Entity);

        }
    }
}
