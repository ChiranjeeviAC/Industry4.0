using Industry4._0.Entities;
using Microsoft.EntityFrameworkCore;

namespace Industry4._0.DBContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<ProductionEntry> ProductionEntries { get; set; }
        public DbSet<Shift> Shifts { get; set; }

    }
}
        
    
