// Import all the necessary namespaces
using JobTracker.Api.Data;
using JobTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Define the namespace for our controllers
namespace JobTracker.Api.Controllers
{
    // [ApiController] enables specific API behaviors like automatic error responses
    [ApiController]
    // [Route] defines the URL pattern for this controller.
    // "api/[controller]" means "api/jobapplications" (it takes the class name prefix)
    [Route("api/[controller]")]
    public class JobApplicationsController : ControllerBase
    {
        // This is a private field to hold our database context
        private readonly ApplicationDbContext _context;

        // This is the constructor
        // We use Dependency Injection (DI) to "inject" the ApplicationDbContext
        // This is the 'service' we registered in Program.cs
        public JobApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Our First Endpoint: GET all job applications ---

        // [HttpGet] attribute specifies this method responds to HTTP GET requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            // This is asynchronous (non-blocking) I/O.
            // We 'await' the database call, which is best practice.
            // _context.JobApplications is the DbSet from our DbContext.
            // .ToListAsync() executes the query against the database.
            return await _context.JobApplications.ToListAsync();
        }

        // --- Our Second Endpoint: POST (Create) a new job application ---

        // [HttpPost] attribute specifies this method responds to HTTP POST requests
        [HttpPost]
        public async Task<ActionResult<JobApplication>> PostJobApplication(JobApplication jobApplication)
        {
            // EF Core will start tracking this new 'jobApplication' object
            _context.JobApplications.Add(jobApplication);

            // This is the command that actually executes the SQL "INSERT" command
            await _context.SaveChangesAsync();

            // 'CreatedAtAction' returns an HTTP 201 "Created" response.
            // This is the RESTful standard. It tells the client where to find
            // the new resource (e.g., in the 'Location' header of the response).
            return CreatedAtAction(nameof(GetJobApplicationById), new { id = jobApplication.Id }, jobApplication);
        }

        // --- Our Third Endpoint: GET a single job application by ID ---
        // This is needed for the 'CreatedAtAction' above, and is generally useful.

        // [HttpGet("{id}")] means it matches "api/jobapplications/5" (for example)
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetJobApplicationById(int id)
        {
            // .FindAsync(id) is a high-performance method to find an entity by its Primary Key
            var jobApplication = await _context.JobApplications.FindAsync(id);

            // If the application doesn't exist, return a standard HTTP 404 Not Found
            if (jobApplication == null)
            {
                return NotFound();
            }

            // Otherwise, return the job application with an HTTP 200 OK
            return jobApplication;
        }
    }
}