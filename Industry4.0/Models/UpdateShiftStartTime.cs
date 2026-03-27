namespace Industry4._0.Models
{
    public class UpdateShiftStartTime
    {
        public int Id { get; set; }
        
        public TimeSpan StartTime { get; set; }
    }
    public class UpdateShiftEndTime
    {
        public int Id { get; set; }

        public TimeSpan EndTime { get; set; }
    }

    public class UpdateShiftNameDto
    {
        public int Id { get; set; }

        public string ShiftName { get; set; }
    }
}
