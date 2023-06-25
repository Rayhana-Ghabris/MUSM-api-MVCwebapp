namespace MUSM_api_MVCwebapp.Dtos
{
    public class UserDto
    {

        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string? Speciality { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool Deleted { get; set; } 

    }
}
