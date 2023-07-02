using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using MUSM_api_MVCwebapp.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MUSM_api_MVCwebap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class PicturesAPIController : ControllerBase
    {
        #region Attributes

        private readonly IWebHostEnvironment _environment;

        private readonly MediaService _mediaService;

        private readonly IMapper _mapper;

        private readonly ApplicationDbContext _db;

        #endregion

        #region Constructor
        public PicturesAPIController(IMapper mapper, ApplicationDbContext db, IWebHostEnvironment environment, MediaService mediaService)
        {
            _mediaService = mediaService;
            _environment = environment;
            _db = db;
            _mapper = mapper;
        }

        #endregion

        #region GetPictureByIdAsync

        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]

        public async Task<IActionResult> GetPictureById([FromRoute] int id)
        {
            if (id <= 0) return BadRequest(new JsonResult("Invalid Id!"));

            var picture = await _db.Pictures.FindAsync(id);

            if (picture == null) return NotFound(new JsonResult("Picture Not Found!"));

            var memory = new MemoryStream();

            try
            {
                using (var stream = new FileStream(picture.Path, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;

                return File(memory, _mediaService.GetContentType(picture.Name), Path.GetFileName(picture.Name));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResult(ex.Message.ToString()));
            }
        }

        #endregion


        #region UploadPicture

        [HttpPost("[action]")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> UploadPicture([FromForm] PictureDto data)
        {
            // To save in Picture name field
            string untrustedFileName = Path.GetFileName(data.files.FileName);

            try
            {
                string path = await _mediaService.UploadMedia(data, "Pictures");

                data.Name = untrustedFileName;
                data.Extension = Path.GetExtension(untrustedFileName);
                data.Path = path;
                data.Size = (int)data.files.Length;

                var newPic = _mapper.Map<Picture>(data);

                if (data.RequestId != null)
                {
                    var findRequest = await _db.Requests.FindAsync(data.RequestId);

                    if (findRequest == null)
                    {
                        try
                        {
                            _mediaService.DeleteMedia(path);

                            return NotFound(new JsonResult("Request Not Found"));
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(new JsonResult(ex.Message.ToString()));
                        }
                    }

                    var createPicture = await _db.Pictures.AddAsync(newPic);

                    await _db.SaveChangesAsync();

                    if (createPicture == null)
                    {
                        try
                        {
                            _mediaService.DeleteMedia(path);

                            return BadRequest(new JsonResult("Failed to create new picture"));
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(new JsonResult(ex.Message.ToString()));
                        }
                    }

                    // If the request was found
                    findRequest.PhotoId = newPic.Id;
    
                    _db.Entry(findRequest).State = EntityState.Modified;
                }                

                await _db.SaveChangesAsync();

                return Created("", new JsonResult("Picture Added Successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResult(ex.Message.ToString()));
            }

        }

        #endregion

        #region UpdatePictureByIdAsync

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequirePublicUserRole")]
        public async Task<IActionResult> UpdatePictureById([FromRoute] int id, [FromForm] PictureDto data)
        {
            if (data.files == null) return BadRequest(new JsonResult("Invalid Input"));

            var findPicture = await _db.Pictures.FindAsync(id);

            if (findPicture == null) return NotFound(new JsonResult("Picture Not Found"));

            try
            {
                _mediaService.DeleteMedia(findPicture.Path);

                // To save in Picture name field
                string untrustedFileName = Path.GetFileName(data.files.FileName);

                try
                {
                    string path = await _mediaService.UploadMedia(data, "Pictures");

                    findPicture.Name = untrustedFileName;
                    findPicture.Extension = Path.GetExtension(untrustedFileName);
                    findPicture.Path = path;
                    findPicture.Size = (int)data.files.Length;

                    _db.Entry(findPicture).State = EntityState.Modified;

                    await _db.SaveChangesAsync();

                    return Ok(new JsonResult("The Picture with id " + id + " is updated"));

                }

                catch (Exception ex)
                {
                    return BadRequest(new JsonResult(ex.Message.ToString()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResult(ex.Message.ToString()));
            }

        }

        #endregion

        /*#region DeletePictureByIdAsync

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<Picture>> DeletePictureById([FromRoute] int id)
        {
            var picture = await _db.Pictures.FindAsync(id);

            if (picture == null)
                return NotFound(new JsonResult("Picture Not Found!"));

            try
            {
                _mediaService.DeleteMedia(picture.Path, "Pictures");

                var delete = _db.Pictures.Remove(picture);

                if (delete == null) return BadRequest(new JsonResult("The Picture is not deleted."));

                await _db.SaveChangesAsync();

                // Finally return the result to client
                return Ok(new JsonResult("The Picture with id " + id + " is deleted."));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResult(ex.Message.ToString()));
            }
        }

        #endregion*/
    }
}