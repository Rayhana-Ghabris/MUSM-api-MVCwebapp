using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using NuGet.Common;

namespace MUSM_api_MVCwebapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsAPIController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserWithRole([FromBody] UserCreationDto UserRegisterDto)
        {
            // Will hold all the errors related to registration
            List<string> errorList = new List<string>();

            var user = new AppUser
            {
                Email = UserRegisterDto.Email,
                UserName = UserRegisterDto.Email,
                PhoneNumber = "7777777",
                FullName = UserRegisterDto.FullName,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, UserRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRegisterDto.Role);
                
                return Created("", new { username = user.UserName, email = user.Email, status = 1, message = "Registration Successful" });
               
            }

            else
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }


            return BadRequest(new JsonResult(errorList));

        }


    }
}
