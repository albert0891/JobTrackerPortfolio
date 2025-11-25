using JobTracker.Api.Data;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobApplication>> GetAllJobsAsync()
        {
            // Business logic goes here (e.g., filtering inactive jobs)
            return await _context.JobApplications.ToListAsync();
        }

        public async Task<JobApplication?> GetJobByIdAsync(int id)
        {
            return await _context.JobApplications.FindAsync(id);
        }

        public async Task<JobApplication> CreateJobAsync(JobApplication job)
        {
            _context.JobApplications.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<bool> UpdateJobStatusAsync(int id, string status)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null) return false;

            // Update the status property
            job.Status = status;
            
            // Mark the entity as modified and save changes
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null) return false;

            _context.JobApplications.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}