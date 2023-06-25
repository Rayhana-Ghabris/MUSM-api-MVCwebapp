using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;
using System.Linq;

namespace MUSM_api_MVCwebapp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        private readonly List<string> Roles = new List<string> {
            "Worker", "PublicUser"
        };

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
        }


        #region Index
        // 1- Order by CreatedAt: ascending: true - descending: false 
        // 2- Search Box: Id, User.FullName, Email
        // 3- Filters:
        //           Roles: "Worker", "PublicUser"
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Index(bool showDeleted, bool ascending, string? searchString, List<string> SelectedRoles)
        {

            List<UserDto> result = new List<UserDto>();
            IQueryable<AppUser> users ;
            var managers = await _userManager.GetUsersInRoleAsync("Manager");

            if (SelectedRoles != null && SelectedRoles.Count == 1)
          {
                 users = (
                      from user in _context.Users
                      join userRole in _context.UserRoles
                      on user.Id equals userRole.UserId
                      join role in _context.Roles
                      on userRole.RoleId equals role.Id
                      where role.Name == SelectedRoles[0]
                      select user);

            }
            else
            {
               
                users = _userManager.Users;
            }



            if (!showDeleted)
            {
                users = users.Where(user => user.Deleted == false);
            }

            //If Search Box is not Empty, apply conditions on the query

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(user => user.Id.Contains(searchString)
                || user.FullName.Contains(searchString)
                || user.Email.Contains(searchString));
            }


            if (!ascending)
            {
                users = users.OrderByDescending(t => t.CreatedAt);
            }

            else
            {
                users = users.OrderBy(t => t.CreatedAt);
            }

            ViewData["SelectedRoles"] = new SelectList(Roles);

           

            foreach (AppUser user in users)
            {
                UserDto userData = _mapper.Map<UserDto>(user);

                var roles = await _userManager.GetRolesAsync(user);

                userData.Role = roles[0];

                result.Add(userData);
            }


            return View(result);
        }
        #endregion


        #region Create
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Create()
        {
            //ViewData["SelectedRoles"] = new SelectList(Roles);

            return View();
        }
        #endregion

        #region CreateUserWithRole

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateUserWithRole(UserCreationDto UserRegisterDto)
        {
            // Will hold all the errors related to registration
            List<string> errorList = new List<string>();

            var user = new AppUser
            {
                FullName = UserRegisterDto.FullName,
                Email = UserRegisterDto.Email,
                UserName = UserRegisterDto.Email,
                Speciality = UserRegisterDto.Speciality,
                PhoneNumber = "7777777",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()

            };
            

            var result = await _userManager.CreateAsync(user, UserRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRegisterDto.Role);

                return RedirectToAction(nameof(Index));

            }

            else
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }


            return View(UserRegisterDto);

        }
        #endregion


        #region Edit

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user== null)
            {
                return NotFound();
            }

            var userRole = await _userManager.GetRolesAsync(user);

            EditUserDto userDto = new EditUserDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = userRole[0],
                Speciality = user.Speciality
            };
          
            return View(userDto);
        }
        #endregion

        #region Edit User

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {
                try {
                    user.FullName = userDto.FullName;
                    user.Email = userDto.Email;
                    user.UserName = userDto.Email;
                    user.Speciality = userDto.Speciality;

                    _context.Entry(user).State = EntityState.Modified;

                    if (!userRole[0].Equals(userDto.Role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, userRole[0]);
                        await _userManager.AddToRoleAsync(user, userDto.Role);

                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDtoExists(userDto.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


                return RedirectToAction(nameof(Index));
            }


            return View(userDto);
        }
        #endregion


        #region GET:ChangePassword
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> ChangePassword(string? id)
        {
            
            if (id == null || _context.Users == null)
            { 
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = await _userManager.GetRolesAsync(user);

            ChangePasswordDto userDto = new ChangePasswordDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = userRole[0]
            };

            return View(userDto);
        }
        #endregion

        #region ChangePassword

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            List<string> errorList = new List<string>();


            if (ModelState.IsValid)
            {

                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, userDto.Password);


                if (result.Succeeded)
                {
                   
                    return RedirectToAction(nameof(Index));

                }

                else
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        errorList.Add(error.Description);
                    }
            }


            return View(userDto);
        }
        #endregion


        #region UndoDelete
        // GET: Users/UndoDelete/5

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UndoDelete(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var userDto = await _context.Users.FindAsync(id);
            if (userDto != null)
            {
                userDto.Deleted = false;

                _context.Entry(userDto).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete User
        // GET: Users/Delete/5

        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var userDto = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (userDto == null)
            {
                return NotFound();
            }

            return View(userDto);
        }
        #endregion

        #region Delete Confirmed
        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var userDto = await _context.Users.FindAsync(id);
            if (userDto != null)
            {
                userDto.Deleted = true;

                _context.Entry(userDto).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region UserDtoExists
        private bool UserDtoExists(string email)
        {
            return (_context.Users?.Any(u => u.Email == email)).GetValueOrDefault();
        }
        #endregion
    }
}
