using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Microsoft.AspNetCore.Mvc;

namespace Industry4._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ShiftController(AppDBContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public IActionResult AddShift(ShiftCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check duplicate shift name
            var existing = _context.Shifts
                .FirstOrDefault(s => s.ShiftName == dto.ShiftName);

            if (existing != null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Shift already exists"
                });
            }

            var shift = new Shift
            {
                ShiftName = dto.ShiftName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.Shifts.Add(shift);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetShiftById),
                new { id = shift.Id },
                new
                {
                    Status = true,
                    Message = "Shift created successfully",
                    Data = shift
                });
        }

        
        [HttpGet("{id}")]
        public IActionResult GetShiftById(int id)
        {
            var shift = _context.Shifts
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.ShiftName,
                    s.StartTime,
                    s.EndTime
                })
                .FirstOrDefault();

            if (shift == null)
                return NotFound(new
                {
                    Status = false,
                    Message = "Shift not found",

                });

            return Ok(new
            {
                Status = true,
                Message = $"Shift at id: {id}",
                Data = shift
            });
        }


        [HttpGet]
        public IActionResult GetAllShift()
        {
            var shifts = _context.Shifts
                .Select(s => new
                {
                    s.Id,
                    s.ShiftName,
                    s.StartTime,
                    s.EndTime
                })
                .ToList();

            if (shifts == null)
                return NotFound(new
                {
                    Status = false,
                    Message = "No Shift Present",

                });

            return Ok(new
            {
                Status = true,
                Message = $"Total number of Shifts are: {shifts.Count}",
                Data = shifts
            });
        }
    }
}