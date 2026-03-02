
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



    }
}
