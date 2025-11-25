using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationsController : ControllerBase
    {
        // Depend on the Interface, not the DbContext
        private readonly IJobService _jobService; 

        public JobApplicationsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // GET: api/JobApplications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }

        // GET: api/JobApplications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetJobApplication(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            return job;
        }

        // POST: api/JobApplications
        [HttpPost]
        public async Task<ActionResult<JobApplication>> PostJobApplication(JobApplication jobApplication)
        {
            var createdJob = await _jobService.CreateJobAsync(jobApplication);

            // Returns 201 Created status
            return CreatedAtAction(nameof(GetJobApplication), new { id = createdJob.Id }, createdJob);
        }

        // PUT: api/JobApplications/status/5
        // Endpoint for drag-and-drop status update
        [HttpPut("status/{id}")]
        public async Task<IActionResult> PutJobApplicationStatus(int id, [FromBody] JobApplicationStatusUpdateDto statusUpdate)
        {
            if (string.IsNullOrEmpty(statusUpdate.Status))
            {
                return BadRequest("Status cannot be empty.");
            }
            
            var success = await _jobService.UpdateJobStatusAsync(id, statusUpdate.Status);

            if (!success)
            {
                return NotFound();
            }

            // Returns 204 No Content
            return NoContent();
        }

        // DELETE: api/JobApplications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var success = await _jobService.DeleteJobAsync(id);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}