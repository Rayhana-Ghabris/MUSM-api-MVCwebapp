﻿using MUSM_api_MVCwebapp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MUSM_api_MVCwebapp.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Title { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Description { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Location { get; set; }

        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        [Display(Name = "Completion Status")]
        public string? CompletionStatus { get; set; } = "Open";

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Priority { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for title is {1}")]
        public string? Category { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Date Completed")]
        public DateTime? DateCompleted { get; set; }
        
        [StringLength(450, ErrorMessage = "Maximum length for title is {1}")]
        public string? WorkerId { get; set; }
        [Display(Name = "Worker")]

        [ForeignKey("WorkerId")]
        public AppUser? Worker { get; set; }
        
        [Range(0, 1000000000)]
        public int? RequestId { get; set; }

        [ForeignKey("RequestId")]
        public RequestModel? Request { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
       
        public bool Deleted { get; set; } = false;
    }
}
