using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MUSM_api_MVCwebapp.Dtos
{
    public class UserCreationDto
    {

        [Required]
        [DataType(DataType.Password)]
        [StringLength(256, ErrorMessage = "Maximum length for password is {1}")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9_]).*$")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

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
