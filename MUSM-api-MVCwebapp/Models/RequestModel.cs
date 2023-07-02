using MUSM_api_MVCwebapp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSM_api_MVCwebapp.Models
{
    public class RequestModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Title { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Description { get; set; }

        [Display(Name = "Approval Status")]
        public string ApprovalStatus { get; set; } = "Under Evaluation";

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Location { get; set; }

        [Range(0, 1000000000)]
        public int? PhotoId { get; set; }
        [Display(Name = "Photo")]

        [ForeignKey("PhotoId")]
        public Picture? Photo { get; set; }


        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Category { get; set; }

   
        [StringLength(450, ErrorMessage = "Maximum length for title is {1}")]
        public string? PublicUserId { get; set; }
        [Display(Name = "Public User")]

        [ForeignKey("PublicUserId")]
        public AppUser? PublicUser { get; set; }

       //new 
        public TaskModel? Task { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        

        public bool Deleted { get; set; } = false;

        [NotMapped]
        [Display(Name = "Votes Count")]
        public int? VotesCount { get; set; }
        


    }
}
