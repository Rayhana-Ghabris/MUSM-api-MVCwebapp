using Microsoft.AspNetCore.Mvc;
using MUSM_api_MVCwebapp.Data;

namespace MUSM_api_MVCwebapp.Services
{
    public class VotesService
    {

        private readonly ApplicationDbContext _db;

        public VotesService(ApplicationDbContext db)
        {
            _db = db;
        }
        public int CountVotesByRequestId(int? id)
        {
            if (id <= 0)
            {
                return 0;
            }

            var countVotes = _db.Votes.Where(r => r.RequestId == id).Count();

            return countVotes;
        }
    }
}
