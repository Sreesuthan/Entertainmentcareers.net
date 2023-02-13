using Entertainmentcareers.net.Shared;
using Microsoft.EntityFrameworkCore;

namespace Entertainmentcareers.net.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<JobType> Jobtypes { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EmploymentTypes> EmploymentTypes { get; set; }
    }
}
