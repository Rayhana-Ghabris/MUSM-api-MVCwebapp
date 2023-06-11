using MUSM_api_MVCwebapp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MUSM_api_MVCwebapp.Models
{
    public class VoteModel
    {
        [Required]
        [StringLength(450, ErrorMessage = "Maximum length for title is {1}")]
        public string? PublicUserId { get; set; }
        [Display(Name = "Public User")]

        [ForeignKey("PublicUserId")]
        public AppUser? PublicUser { get; set; }


        [Required]
        [Range(0, 1000000000)]
        public int? RequestId { get; set; }
        [Display(Name = "Request")]

        [ForeignKey("RequestId")]
        public RequestModel? Request { get; set; }


    }
}
