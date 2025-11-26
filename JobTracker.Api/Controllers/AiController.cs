using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly AiService _aiService;

        public AiController(AiService aiService)
        {
            _aiService = aiService;
        }

        // POST: api/Ai/analyze/5
        [HttpPost("analyze/{jobId}")]
        public async Task<IActionResult> AnalyzeJob(int jobId)
        {
            try 
            {
                var analysis = await _aiService.AnalyzeJobAsync(jobId);

                if (analysis == null)
                {
                    return NotFound($"Job {jobId} not found or analysis failed.");
                }

                return Ok(analysis);
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}