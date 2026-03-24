namespace Industry4._0.Models
{
    public class UpdateShift
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
