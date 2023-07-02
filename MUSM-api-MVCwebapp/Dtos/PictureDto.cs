using System.ComponentModel.DataAnnotations;
namespace MUSM_api_MVCwebapp.Dtos

{
    public class PictureDto : MediaDto
    {
        [Range(0, 1000000000)]
        public int? RequestId { get; set; }

       
    }
}
