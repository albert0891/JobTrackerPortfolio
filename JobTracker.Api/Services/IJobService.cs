using JobTracker.Api.Models;

namespace JobTracker.Api.Services
{
    // Interface for Job application business logic
    public interface IJobService
    {
        Task<IEnumerable<JobApplication>> GetAllJobsAsync();
        Task<JobApplication?> GetJobByIdAsync(int id);
        Task<JobApplication> CreateJobAsync(JobApplication job);
        Task<bool> UpdateJobStatusAsync(int id, string status);
        Task<bool> DeleteJobAsync(int id);
    }
}