using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using MUSM_api_MVCwebapp.Services;
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

        private readonly VotesService _votesService;

        public VotesAPIController(ApplicationDbContext db, VotesService votesService )
        {
            _db = db;
            _votesService = votesService;
        }

        #region IsVoted

        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> IsVoted([FromRoute] int? id)
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
        #endregion

        #region VoteOnRequest

        [HttpPost("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> VoteOnRequest([FromBody] VoteModel vote)
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

            return Ok("Vote Created Successfully");

        }
        #endregion

        #region CountVotes

        //CountVote([route]idofrequest)
        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> CountVotes([FromRoute] int? id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var countVotes = _votesService.CountVotesByRequestId(id);

            return Ok(countVotes);
        }
        #endregion

        #region RemoveVote

        [HttpDelete("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<ActionResult> RemoveVote([FromBody] VoteModel vote)
        {

            var request = await _db.Requests.FindAsync(vote.RequestId);
                if (request == null)
                {
                    return NotFound();
                }

                var user = await _db.Users.FindAsync(vote.PublicUserId);
                if (user == null || !vote.PublicUserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                {
                    return NotFound();
                }

                var VoteToBeRemoved = await _db.Votes.FindAsync(vote.RequestId, vote.PublicUserId);

                if (VoteToBeRemoved == null)
                {
                return NotFound(VoteToBeRemoved);
                }
                 _db.Votes.Remove(VoteToBeRemoved);

                await _db.SaveChangesAsync();

                return Ok("Vote Removed Succefully");

            }
        #endregion


    }
}
