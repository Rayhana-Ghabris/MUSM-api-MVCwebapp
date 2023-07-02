using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Models;
using System.ComponentModel.DataAnnotations;
namespace MUSM_api_MVCwebapp.Dtos
{
    public class RequestDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ApprovalStatus { get; set; } = "Under Evaluation";

        public string? Location { get; set; }

        public int? PhotoId { get; set; }       

        public Picture? Photo { get; set; }

        public string? Category { get; set; }

        public string? PublicUserId { get; set; }

        public AppUser? PublicUser { get; set; }

        public TaskModel? Task { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; }

    }
}
