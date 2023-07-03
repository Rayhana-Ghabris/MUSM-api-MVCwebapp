using MUSM_api_MVCwebapp.Models;

namespace MUSM_api_MVCwebapp.Helpers
{
    public class AppSettings
    {
        //Properties for JWT Token Signature
        public string Site { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }

        // Media Validation
        public string[] permittedExtensions = { ".png", ".jpeg", ".jpg"};
        public int FileSizeLimit { get; set; }
        public string FileNameRegex { get; set; }
    }
}
