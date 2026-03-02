using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Entities
{
    public class Machine
    {
        [Key]
        public int Id { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public bool IsActive { get; set; }
    }
}
