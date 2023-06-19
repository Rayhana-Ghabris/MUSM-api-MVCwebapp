using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using System.Net;
using System.Security.Claims;

namespace MUSM_api_MVCwebapp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VotesAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;

        public VotesAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> GetVoted([FromRoute] int? id)
        {

            if (id == null || _db.Votes == null)
            {
                return NotFound();
            }

            var request = await _db.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vote = await _db.Votes.FindAsync(id, userId);


            if (vote != null)
            {
                return Ok(true);
            }
            return Ok(false);

        }

        [HttpPost("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> VoteToRequest([FromBody] VoteModel vote)
        {

            var request = await _db.Requests.FindAsync(vote.RequestId);
            if (request == null)
            {
                return NotFound();
            }

            var user =  await _db.Users.FindAsync(vote.PublicUserId);
            if (user == null || !vote.PublicUserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return NotFound();
            }


              var createVote = await _db.Votes.AddAsync(vote);

            await _db.SaveChangesAsync();

            return Ok("Vote Created Succefully");

        }


        //CountVote([route]idofrequest)
        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> CountVotes([FromRoute] int? id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var vote = await _db.Votes
              .FindAsync(id);

            if (vote == null) return NotFound();

            var countVotes = _db.Votes.Count();

            await _db.SaveChangesAsync();

            return Ok(countVotes);
        }


        //DeleteVote([route]idofrequest)
        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> DeleteVote([FromRoute] int? id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var vote = await _db.Votes
              .FindAsync(id);

            if (!vote.PublicUserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                 || vote == null)
            {
                return NotFound();
            }

            _db.Votes.Remove(vote);

            await _db.SaveChangesAsync();

            return Ok("Successfully deleted");

        }

        



    }
}
