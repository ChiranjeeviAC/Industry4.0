using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.PortableExecutable;

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


        [HttpGet("TotalOKCount")]
        public IActionResult TotalOKCount()
        {
            var totalOk = _context.ProductionEntries.Sum(p => p.OkParts);
            return Ok(new
            {
                TotalOkCount = totalOk
            });
        }

        [HttpGet("TotalNCCount")]
        public IActionResult TotalNCCount()
        {
            var totalNC = _context.ProductionEntries.Sum(p => p.NcParts);
            return Ok(new
            {
                TotalOkCount = totalNC
            });
        }

        [HttpGet("TotalOKCountFromMachine")]
        public IActionResult TotalOKCountFromMachine(int machineId)
        {
            var totalOk = _context.ProductionEntries.Where(p => p.MachineId == machineId).ToList();
            var totalokM = totalOk.Sum(p => p.OkParts);
            var totalncM = totalOk.Sum(p => p.NcParts);
            return Ok(new
            {
                machine = machineId,
                TotalOkParts = totalokM,
                TotalNcParts = totalncM,
                TotalProduction = totalokM + totalncM
            });
        }

        [HttpGet("TotalOkNcCountFromMachineFromTodate")]
        public IActionResult TotalOKCountFromMachinedate(int machineId, DateTime from, DateTime to)
        {
            var production = _context.ProductionEntries
        .Where(p => p.MachineId == machineId
        && p.EntryTime >= from
        && p.EntryTime <= to)
        .ToList();

            var totalokM = production.Sum(p => p.OkParts);
            var totalncM = production.Sum(p => p.NcParts);
            return Ok(new
            {
                machine = machineId,
                fromDate = from,
                toDate = to,
                TotalOkParts = totalokM,
                TotalNcParts = totalncM,
                TotalProduction = totalokM + totalncM
            });
        }

        [HttpGet("machine-summary")]
        public IActionResult machinesummary()
        {
            var result = (
        from p in _context.ProductionEntries
        join m in _context.Machines on p.MachineId equals m.Id
        group p by new
        {
            m.MachineCode,
            m.MachineName
        }
        into g
        select new
        {
            MachineCode = g.Key.MachineCode,
            MachineName = g.Key.MachineName,
            TotalOKParts = g.Sum(x => x.OkParts),
            TotalNCParts = g.Sum(x => x.NcParts),
            TotalParts = g.Sum(x => x.OkParts + x.NcParts)
        }).ToList();

            if (result.Count == 0)
            {
                return NoContent();
            }

            return Ok(new 
            {
                Status = true,
                Message = "Details of Production According to Machine",
                Data = result
            });
        }



        [HttpGet("operator-performance")]
        public IActionResult operatorperformance()
        {
            var result = (
        from p in _context.ProductionEntries
        join u in _context.AppUsers on p.UserId equals u.Id
        group p by new
        {
            u.EmployeeId
           
        }
        into g
        select new
        {
            EmployeeId = g.Key.EmployeeId,
            TotalOKParts = g.Sum(x => x.OkParts),
            TotalNCParts = g.Sum(x => x.NcParts),
            TotalParts = g.Sum(x => x.OkParts + x.NcParts),
            Performance = (g.Sum(x => x.OkParts) /(double)g.Sum(x => x.OkParts + x.NcParts)) * 100
        }).ToList();


            if (result.Count == 0)
            {
                return NoContent();
            }

            return Ok(new
            {
                Status = true,
                Message = "Details of Production According to User",
                Data = result
            });
        }


        [HttpGet("shift-report/{shiftId}")]
        public IActionResult ShiftReport(int shiftId)
        {
            var result = (
                from p in _context.ProductionEntries
                join m in _context.Machines on p.MachineId equals m.Id
                join u in _context.AppUsers on p.UserId equals u.Id
                join s in _context.Shifts on p.ShiftId equals s.Id
                where p.ShiftId == shiftId
                group p by new
                {
                    p.ShiftId,
                    s.ShiftName,
                    m.MachineName,
                    u.EmployeeId
                }
                into g
                select new
                {
                    ShiftId = g.Key.ShiftId,
                    Shift = g.Key.ShiftName,
                    Machine = g.Key.MachineName,
                    EmployeeID = g.Key.EmployeeId,
                    TotalOKParts = g.Sum(x => x.OkParts),
                    TotalNCParts = g.Sum(x => x.NcParts),
                    TotalParts = g.Sum(x => x.OkParts + x.NcParts),
                    Performance = (g.Sum(x => x.OkParts) /
                                  (double)g.Sum(x => x.OkParts + x.NcParts)) * 100
                }).ToList();

            if (!result.Any())
                return NoContent();

            return Ok(new
            {
                Status = true,
                Message = "Details of Production According to Shift",
                Data = result
            });
        }

        [HttpGet("daily")]
        public IActionResult daily( DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            var production = _context.ProductionEntries
        .Where(p => p.EntryTime >= start && p.EntryTime < end)
        .ToList();

            if (production == null)
            {
                return NoContent();
            }
            
            return Ok(new
            {
                
                Date = date,
                TotalOkParts = production.Sum(p => p.OkParts),
                TotalNcParts = production.Sum(p => p.NcParts),
                TotalProduction = production.Sum(p => p.OkParts) + production.Sum(p => p.NcParts)
            });
        }

        [HttpGet("daily{machineId}")]
        public IActionResult dailyMachimeSummary(int machineId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            var production = _context.ProductionEntries
        .Where(p => p.MachineId == machineId && p.EntryTime >= start && p.EntryTime < end)
        .ToList();

            if (production == null)
            {
                return NoContent();
            }

            return Ok(new
            {
                MachineId = machineId,
                Date = date,
                TotalOkParts = production.Sum(p => p.OkParts),
                TotalNcParts = production.Sum(p => p.NcParts),
                TotalProduction = production.Sum(p => p.OkParts) + production.Sum(p => p.NcParts)
            });
        }


        [HttpGet("top-machine")]
        public IActionResult TopMachine()
        {
            var result = (
                from p in _context.ProductionEntries
                join m in _context.Machines on p.MachineId equals m.Id
                group p by new
                {
                    m.MachineName
                }
                into g
                select new
                {
                    Machine = g.Key.MachineName,
                    TotalProduction = g.Sum(x => x.OkParts + x.NcParts)
                }
            )
            .OrderByDescending(x => x.TotalProduction)
            .FirstOrDefault();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(new
            {
                Status = true,
                Message = "Top performing machine",
                Data = result
            });
        }



        [HttpPost("Production-by-Machine-User-PerCycle")]
        public IActionResult ProductionbyMachineUserPerCycle(GetMachineandUserProduction dto)
        {
           

            var production = _context.ProductionEntries
            .Where(p => p.MachineId == dto.Mid && p.UserId == dto.Uid
            && p.EntryTime >= dto.from
            && p.EntryTime <= dto.to)
            .ToList();

            if (production.Count == 0)
            {
                return NotFound();
            }
            
            return Ok(new
            {
                Status = true,
                Message = "Data fetch Secussfully",
                Data = new
                {
                    machine = dto.Mid,
                    user = dto.Uid,
                    fromDate = dto.from,
                    toDate = dto.to,
                    TotalOkParts = production.Sum(p => p.OkParts),
                    TotalNcParts = production.Sum(p => p.NcParts),
                    TotalProduction = production.Sum(p => p.OkParts) + production.Sum(p => p.NcParts)
                }
            });
        }





    }
}