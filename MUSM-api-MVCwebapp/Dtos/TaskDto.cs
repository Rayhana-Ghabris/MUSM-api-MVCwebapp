using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Models;

namespace MUSM_api_MVCwebapp.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    
        public string? Location { get; set; }
  
        public string? CompletionStatus { get; set; } = "Open";

        public string? Priority { get; set; }

        public string? Category { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? DateCompleted { get; set; }

        public string? WorkerId { get; set; }
       
        public AppUser? Worker { get; set; }


        public int? RequestId { get; set; }

        
        public RequestModel? Request { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; } = false;
    }
}
