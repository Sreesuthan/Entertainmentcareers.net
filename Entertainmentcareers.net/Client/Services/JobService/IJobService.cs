using Entertainmentcareers.net.Shared;

namespace Entertainmentcareers.net.Client.Services.JobService
{
    public interface IJobService
    {
        List<Job> jobs { get; set; }
        Task<Job> GetSingleJob(int id);
        Task CreateJob(Job job);
        Task UpdateJob(Job job);
    }
}
