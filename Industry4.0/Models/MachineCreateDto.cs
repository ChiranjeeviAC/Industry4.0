using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Models
{
    public class MachineCreateDto
    {
        
        public string MachineCode { get; set; }

        
        public string MachineName { get; set; }
        public bool IsActive { get; set; }
    }
}