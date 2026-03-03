using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace Industry4._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly AppDBContext _context;

        public MachineController(AppDBContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public IActionResult AddMachine(MachineCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _context.Machines
                .FirstOrDefault(m => m.MachineCode == dto.MachineCode);

            if (existing != null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Machine code already exists"
                });
            }

            var machine = new Machine
            {
                MachineCode = dto.MachineCode,
                MachineName = dto.MachineName,
                IsActive = dto.IsActive
            };

            _context.Machines.Add(machine);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetMachineById),
                new { id = machine.Id },
                new
                {
                    Status = true,
                    Message = "Machine added successfully",
                    Data = machine
                });
        }


        [HttpGet]
        public IActionResult GetAllMachines()
        {
            var machines = _context.Machines
                .Select(m => new
                {
                    m.Id,
                    m.MachineCode,
                    m.MachineName,
                    m.IsActive
                })
                .ToList();

            return Ok(machines);
        }

        
        [HttpGet("{id}")]
        public IActionResult GetMachineById(int id)
        {
            var machine = _context.Machines
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    m.Id,
                    m.MachineCode,
                    m.MachineName,
                    m.IsActive
                })
                .FirstOrDefault();

            if (machine == null)
                return NotFound("Machine not found");

            return Ok(machine);
        }


        [HttpGet("GetAllActiveMachine")]
        public IActionResult GetAllActiveMachine()
        {
            var machines = _context.Machines
                .Where(i => i.IsActive )
                .Select(i => new
                {
                    i.Id,
                    i.MachineCode,
                    i.MachineName,

                }).ToList();

            if( machines.Count == 0)
            {
                return NotFound(
                    new
                    {
                        Status = false,
                        Message = "No Active Machine found"
                    });
            }
            return Ok(new
            {
                Status = true,
                Message = $"Number of Active Machine are {machines.Count}",
                Data = machines
            });
        }

        //To Deactivate Machine
        [HttpPatch("DeactivateMachine")]
        public IActionResult DeactivateMachine(int id)
        {
            var machine = _context.Machines.Where(m => m.Id == id).FirstOrDefault();
            if (machine == null) return BadRequest(new
            {
                Status = false,
                Message = "Machine Not Found."
            });
            if (!machine.IsActive) return BadRequest(new
            {
                Status = false,
                Message = "Machine already deactivated."
            });


            machine.IsActive = false;
            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Machine get deactivated.",
                Data = machine
            });
        }


        //To Delete Machine
        [HttpDelete("RemoveMachine")]
        public IActionResult RemoveMachine(int id)
        {
            var machine = _context.Machines.Where(m => m.Id == id).FirstOrDefault();
            if (machine == null) return BadRequest(new
            {
                Status = false,
                Message = "Machine Not Found."
            });
            
            _context.Machines.Remove(machine);
            _context.SaveChanges();
            return Ok(new
            {
                Status = true,
                Message = "Machine get Deleted.",
                Data = machine
            });
        }




    }


}