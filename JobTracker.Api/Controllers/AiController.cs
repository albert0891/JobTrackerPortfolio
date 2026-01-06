using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> AnalyzeJob(int jobId, [FromForm] IFormFile resume)
        {
            if (resume == null || resume.Length == 0)
            {
                return BadRequest("Please upload a resume PDF.");
            }

            try
            {
                // Open the read stream directly from the uploaded file
                using var stream = resume.OpenReadStream();

                var analysis = await _aiService.AnalyzeJobAsync(jobId, stream);

                if (analysis == null)
                {
                    return NotFound($"Analysis failed.");
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