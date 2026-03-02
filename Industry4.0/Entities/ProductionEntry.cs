using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Entities
{
    public class ProductionEntry
    {
        [Key]
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int ShiftId { get; set; }
        public int UserId { get; set; }
        public int OkParts { get; set; }
        public int NcParts { get; set; }
        public DateTime EntryTime { get; set; }
    }
}
