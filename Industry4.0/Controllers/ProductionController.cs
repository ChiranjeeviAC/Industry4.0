using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Industry4._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ProductionController(AppDBContext context)
        {
            _context = context;
        }



        [HttpPost]
        public IActionResult AddProduction(ProductionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ProdEntryAlredy = _context.ProductionEntries.Where(i => i.JobId == dto.JobId).FirstOrDefault();

            if (ProdEntryAlredy != null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Production entry aleardy present",

                }
                    );
            }

            var machine = _context.Machines.FirstOrDefault(m => m.Id == dto.MachineId);
            if (machine == null)
                return BadRequest("Invalid Machine");


            var shift = _context.Shifts.FirstOrDefault(s => s.Id == dto.ShiftId);
            if (shift == null)
                return BadRequest("Invalid Shift");


            var user = _context.AppUsers.FirstOrDefault(u => u.Id == dto.UserId);
            if (user == null)
                return BadRequest("Invalid User");

            var production = new ProductionEntry
            {
                MachineId = dto.MachineId,
                JobId = dto.JobId,
                ShiftId = dto.ShiftId,
                UserId = dto.UserId,
                OkParts = dto.OkParts,
                NcParts = dto.NcParts,
                EntryTime = DateTime.Now
            };

            _context.ProductionEntries.Add(production);
            _context.SaveChanges();

            return Ok(new
            {
                Status = true,
                Message = "Production entry added successfully",
                Data = production
            });
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = (
                from p in _context.ProductionEntries
                join m in _context.Machines on p.MachineId equals m.Id
                join u in _context.AppUsers on p.UserId equals u.Id
                join s in _context.Shifts on p.ShiftId equals s.Id
                select new
                {
                    p.Id,
                    Machine = m.MachineName,
                    Shift = s.ShiftName,
                    Operator = u.EmployeeId,
                    p.OkParts,
                    p.NcParts,
                    p.EntryTime,
                    p.JobId
                }
                ).ToList();
            return Ok(new
            {
                Status = true,
                Message = $"Number of Production entry are {result.Count}",
                Data = result
            });
        }

        [HttpGet("Shift/{ShiftId}")]
        public IActionResult GetByShift(int shiftId)
        {
            var result = _context.ProductionEntries.Where(i => i.ShiftId == shiftId).ToList();

            if (result.Count == 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"No Production entry at Shift fount of ShiftId: {shiftId}"
                });
            }

            return Ok(new
            {
                Status = true,
                Message = $" Production entry at Shift of ShiftId: {shiftId}",
                Data = result
            });
        }



        [HttpGet("GetByProductId/{PrudID}")]
        public IActionResult GetByProductId(int PrudID)
        {
            var result = _context.ProductionEntries.Where(i => i.Id == PrudID).FirstOrDefault();

            if (result == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = $"No Production entry at Shift fount of ShiftId: {PrudID}"
                });
            }

            return Ok(new
            {
                Status = true,
                Message = $" Production entry at Shift of ShiftId: {PrudID}",
                Data = result
            });
        }

        [HttpDelete("ByJobId")]
        public IActionResult DeleteProduction(string jobId)
        {
            var res = _context.ProductionEntries.Where(i => i.JobId == jobId).FirstOrDefault();
            if (res == null)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = $"JobID not found for JobID: {jobId}"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = $"Production Entry deleted of JobID: {jobId}",
                Data = res
            });
        }

        



    }
}