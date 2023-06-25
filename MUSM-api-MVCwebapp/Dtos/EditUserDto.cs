using System.ComponentModel.DataAnnotations;

namespace MUSM_api_MVCwebapp.Dtos
{
    public class EditUserDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256, ErrorMessage = "Maximum length for email is {1}")]
        public string Email { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for role is {1}")]
        public string Role { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Maximum length for full name is {1}")]
        [RegularExpression("^[A-Za-z]+$")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string? Speciality { get; set; }
    }
}
