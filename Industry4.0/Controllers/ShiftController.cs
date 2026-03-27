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

        [HttpDelete]
        public IActionResult DeleteShift(int id) { 
            var shift = _context.Shifts.Where(s => s.Id == id).FirstOrDefault();
            if (shift == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"Shift not for id{id}" 
                });
            }
            _context.Shifts.Remove(shift);
            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Shift deleted secussfully",
                Data = shift
            });
        }


        [HttpPut]
        public IActionResult UpdateShift(UpdateShift dto)
        {
            var shift = _context.Shifts.Where(s => s.Id == dto.Id).FirstOrDefault();
            if (shift == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"Shift not for id{dto.Id}"
                });
            }
            shift.ShiftName = dto.ShiftName;
            shift.StartTime = dto.StartTime;
            shift.EndTime = dto.EndTime;
            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Shift Updated secussfully",
                Data = shift
            });
        }
        [HttpPatch]
        public IActionResult UpdateShiftStartTime(UpdateShiftStartTime dto)
        {
            var shift = _context.Shifts.Where(s => s.Id == dto.Id).FirstOrDefault();
            if (shift == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"Shift not for id{dto.Id}"
                });
            }
            
            shift.StartTime = dto.StartTime;
            
            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Shift start time Updated secussfully",
                Data = shift
            });
        }
        [HttpPatch("UpdateShiftStartTime")]
        public IActionResult UpdateShiftStartTime(UpdateShiftEndTime dto)
        {
            var shift = _context.Shifts.Where(s => s.Id == dto.Id).FirstOrDefault();
            if (shift == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"Shift not for id{dto.Id}"
                });
            }

            shift.StartTime = dto.EndTime;

            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Shift end time Updated secussfully",
                Data = shift
            });
        }

        [HttpPatch("UpdateShiftName")]
        public IActionResult UpdateShiftName(UpdateShiftNameDto dto)
        {
            var shift = _context.Shifts.Where(s => s.Id == dto.Id).FirstOrDefault();
            if (shift == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"Shift not for id{dto.Id}"
                });
            }

            shift.ShiftName = dto.ShiftName;

            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Shift start time Updated secussfully",
                Data = shift
            });
        }
    }
}