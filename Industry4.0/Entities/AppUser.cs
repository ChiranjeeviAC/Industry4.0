using System.ComponentModel.DataAnnotations;

namespace Industry4._0.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
