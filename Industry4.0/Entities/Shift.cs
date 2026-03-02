using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Entities
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}


