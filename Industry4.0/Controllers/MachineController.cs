using Industry4._0.DBContext;
using Industry4._0.Entities;
using Industry4._0.Models;
using Microsoft.AspNetCore.Mvc;

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

       
    }
}