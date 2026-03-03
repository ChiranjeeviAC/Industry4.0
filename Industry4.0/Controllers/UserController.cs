
using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Industry4._0.Models.Industry4._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Industry4._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.AppUsers
                .Select(u => new
                {
                    u.Id,
                    u.EmployeeId,
                    u.Role,
                    u.IsActive
                })
                .ToList();

            return Ok(users);
        }

        //  GET USER BY EMPLOYEE ID
        [HttpGet("{employeeId}")]
        public IActionResult Get(string employeeId)
        {
            var user = _context.AppUsers
                .Where(u => u.EmployeeId == employeeId)
                .Select(u => new
                {
                    u.Id,
                    u.EmployeeId,
                    u.Role,
                    u.IsActive
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }






        [HttpPost("register")]
        public IActionResult Register(RegisterUserModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _context.AppUsers
                .FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

            if (existing != null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "User already exists"
                });
            }

            var newUser = new AppUser
            {
                EmployeeId = user.EmployeeId,
                Role = user.Role,
                IsActive = true
            };

            var hasher = new PasswordHasher<AppUser>();

            var auth = new UserAuthDelails
            {
                EmployeeId = user.EmployeeId,
                Password = hasher.HashPassword(newUser, user.Password)
            };

            _context.AppUsers.Add(newUser);
            _context.UserAuthDelails.Add(auth);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(Get),
                new { employeeId = newUser.EmployeeId },
                new
                {
                    Status = true,
                    Message = "User registered successfully",
                    Data = new
                    {
                        newUser.Id,
                        newUser.EmployeeId,
                        newUser.Role,
                        newUser.IsActive
                    }
                });
        }



        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {

            var user = _context.AppUsers
                .FirstOrDefault(x => x.EmployeeId == model.EmployeeId);

            if (user == null)
            {
                return Unauthorized("Invalid Employee ID or Password");
            }

            if (!user.IsActive)
            {
                return Unauthorized("User is inactive");
            }


            var auth = _context.UserAuthDelails
                .FirstOrDefault(x => x.EmployeeId == model.EmployeeId);

            if (auth == null)
            {
                return Unauthorized("Authentication details not found");
            }


            var hasher = new PasswordHasher<AppUser>();

            var result = hasher.VerifyHashedPassword(
                user,
                auth.Password,
                model.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid Employee ID or Password");
            }


            return Ok(new
            {
                Message = "Login successful",
                user.EmployeeId,
                user.Role
            });
        }





        [HttpGet("active")]
        public IActionResult GetActiveUsers()
        {
            var activeUsers = _context.AppUsers
                .Where(u => u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.EmployeeId,
                    u.Role,
                    u.IsActive
                })
                .ToList();

            if (!activeUsers.Any())
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "No active users found"
                });
            }

            return Ok(new
            {
                Status = true,
                Count = activeUsers.Count,
                Data = activeUsers
            });
        }


        [HttpPut("C/{employeeId}")]
        public IActionResult DeactivateUser(string employeeId)
        {
            var user = _context.AppUsers
                .FirstOrDefault(u => u.EmployeeId == employeeId);

            if (user == null)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "User not found"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "User is already inactive"
                });
            }

            user.IsActive = false;

            _context.SaveChanges();

            return Ok(new
            {
                Status = true,
                Message = "User deactivated successfully",
                Data = new
                {
                    user.EmployeeId,
                    user.Role,
                    user.IsActive
                }
            });
        }

        [HttpDelete]

        public IActionResult DeleteUser(string employeeId)
        {
            var user = _context.AppUsers
                .FirstOrDefault(u => u.EmployeeId == employeeId);

            if (user == null)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "User not found"
                });
            }




            _context.AppUsers.Remove(user);
            _context.SaveChanges();

            return Ok(new
            {
                Status = true,
                Message = "User Deleted successfully",
                Data = new
                {
                    user.EmployeeId,
                    user.Role,
                    user.IsActive
                }
            });

        }
    }
}
