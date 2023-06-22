using System.ComponentModel.DataAnnotations;

namespace MUSM_api_MVCwebapp.Dtos
{
    public class ChangePasswordDto
    {

        [Required]
        [DataType(DataType.Password)]
        [StringLength(256, ErrorMessage = "Maximum length for password is {1}")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9_]).*$")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

       
        public string Email { get; set; }

       
        public string Role { get; set; }

       
        public string FullName { get; set; }
    }
}
