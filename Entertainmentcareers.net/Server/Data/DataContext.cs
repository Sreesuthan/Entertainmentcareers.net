using Entertainmentcareers.net.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entertainmentcareers.net.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ApplicationUser> Members { get; set; } 
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<JobType> Jobtypes { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EmploymentTypes> EmploymentTypes { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Alert> AlertSubscribers { get; set; }
    }
}
