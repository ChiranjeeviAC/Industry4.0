using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Models
{
    public class ProductionCreateDto
    {
       
        public int MachineId { get; set; }

        public string JobId { get; set; }
        public int ShiftId { get; set; }

        
        public int UserId { get; set; }

        
        public int OkParts { get; set; }

        
        public int NcParts { get; set; }
    }
}