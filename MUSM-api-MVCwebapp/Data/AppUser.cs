using Microsoft.AspNetCore.Identity;
using MUSM_api_MVCwebapp.Models;
using System.ComponentModel.DataAnnotations;

namespace MUSM_api_MVCwebapp.Data
{
    public class AppUser: IdentityUser
    {
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? FullName { get; set; }

        /*public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; } = false;*/

        public List<TaskModel>? Tasks { get; set; }

        public List<RequestModel>? Requests { get; set; }

    }
}
