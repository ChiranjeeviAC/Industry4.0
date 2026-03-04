using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Models
{
    public class ShiftCreateDto
    {
        
        public string ShiftName { get; set; }

        
        public TimeSpan StartTime { get; set; }

        
        public TimeSpan EndTime { get; set; }

       
    }
}