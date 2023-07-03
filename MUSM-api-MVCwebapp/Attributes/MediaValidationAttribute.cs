using Microsoft.AspNetCore.Http;
using MUSM_api_MVCwebapp.Services;
using System.ComponentModel.DataAnnotations;

namespace MUSM_api_MVCwebapp.Attributes
{
    public class MediaValidationAttribute : ValidationAttribute
    {
        public MediaValidationAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MediaService _mediaService = (MediaService)validationContext.GetService(typeof(MediaService));

            var file = value as IFormFile;

            if (file != null)
            {
                if (!_mediaService.ValidateFileName(file))
                    return new ValidationResult(_mediaService.GetFileNameErrorMessage());

                if (!_mediaService.ValidateFileExtension(file))
                    return new ValidationResult(_mediaService.GetFileExtensionErrorMessage());

                if (!_mediaService.ValidateFileSize(file))
                    return new ValidationResult(_mediaService.GetFileSizeErrorMessage());
            }

            return ValidationResult.Success;
        }


    }
}
