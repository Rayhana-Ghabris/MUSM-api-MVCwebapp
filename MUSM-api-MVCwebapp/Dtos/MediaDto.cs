using Microsoft.AspNetCore.Http;
using MUSM_api_MVCwebapp.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MUSM_api_MVCwebapp.Dtos
{
    public class MediaDto
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [MediaValidation]
        public IFormFile files { get; set; }

        [StringLength(256, ErrorMessage = "Maximum length for text value is {1}")]
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "Maximum length for category value is {1}")]
        public string? Extension { get; set; }

        [StringLength(int.MaxValue, ErrorMessage = "Maximum length for text value is {1}")]
        public string? Path { get; set; }

        [Range(0, 1000000000)]
        public int? Size { get; set; }
    }
}
