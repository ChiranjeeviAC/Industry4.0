
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
            
            var existing = _context.AppUsers
                .FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

            if (existing != null)
            {
                return BadRequest("User already exists");
            }

            
            var newUser = new AppUser
            {
                EmployeeId = user.EmployeeId, 
                Role = user.Role,
                IsActive = true
            };

            var hasher = new PasswordHasher<AppUser>();


            var aut = new UserAuthDelails
            {
                EmployeeId = user.EmployeeId
            };
            aut.Password = hasher.HashPassword(newUser, user.Password);




            _context.AppUsers.Add(newUser);

            _context.UserAuthDelails.Add(aut);


            _context.SaveChanges();

            return Created(nameof(Get), user);
        }

    }
}
