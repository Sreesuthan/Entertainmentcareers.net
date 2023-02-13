using Entertainmentcareers.net.Shared;

namespace Entertainmentcareers.net.Client.Services.JobService
{
    public interface IJobService
    {
        List<Job> jobs { get; set; }
        Task CreateJob(Job job);
    }
}
