using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Helpers;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MUSM_api_MVCwebapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppSettings _appSettings;

        public AccountsAPIController(ApplicationDbContext context, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IOptions<AppSettings> appSettings) {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
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
                
                return Created("", new { id = user.Id, username = user.UserName, email = user.Email, status = 1, message = "Registration Successful" });
               
            }

            else
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }


            return BadRequest(new JsonResult(errorList));

        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto LoginDto)
        {
            // Get the User from Database
            var user = await _userManager.FindByEmailAsync(LoginDto.Email);

            if (user == null) return Unauthorized(new { LoginError = "Please check the login credentials - Invalid Username/Password" });

            //check user password validation
            var checkPasswordTask = _userManager.CheckPasswordAsync(user, LoginDto.Password);

            //generate JWT key
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            if (await checkPasswordTask)
            {
                //get roles for the user
                var roles = await _userManager.GetRolesAsync(user);

                if (roles == null) { return Unauthorized(new { LoginError = "Please check the login credentials - Invalid Username/Password" }); }

                // Generate Token

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id), // User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()), // User.FindFirst(ClaimTypes.Role).Value;
                        new Claim(ClaimTypes.Email, user.Email),//This will be User.FindFirst(ClaimTypes.Email).Value;
                        new Claim("LoggedOn", DateTime.Now.ToString()),

                     }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(token), email = user.Email, name = user.FullName });

            }

            // return error
            ModelState.AddModelError("", "Username/Password was not Found");
            return Unauthorized(new { LoginError = "Please Check the Login Credentials - Ivalid Username/Password" });

        }


    }
}
