namespace Industry4._0.Models
{
    public class ShiftReportModel
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public int TotalOk { get; set; }
        public int TotalNc { get; set; }
        public int TotalParts { get; set; }
    }
}
