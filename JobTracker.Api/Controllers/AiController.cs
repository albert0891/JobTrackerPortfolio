using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Channels;

namespace JobTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("GeminiPolicy")]
    public class AiController : ControllerBase
    {
        private readonly Channel<AnalysisRequest> _channel;

        public AiController(Channel<AnalysisRequest> channel)
        {
            _channel = channel;
        }

        // POST: api/Ai/analyze/{jobId}
        [HttpPost("analyze/{jobId}")]
        public async Task<IActionResult> AnalyzeJob(int jobId, IFormFile resume)
        {
            if (resume == null || resume.Length == 0) return BadRequest("Please upload a resume PDF.");

            try
            {
                var memoryStream = new MemoryStream();
                await resume.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                await _channel.Writer.WriteAsync(new AnalysisRequest(jobId, memoryStream, AnalysisRequestType.Analyze));

                return Accepted(new { status = "Queued", message = "Analysis started." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // POST: api/Ai/generate-resume/{jobId}
        [HttpPost("generate-resume/{jobId}")]
        public async Task<IActionResult> GenerateResume(int jobId)
        {
            // Resume Stream is null because we use the one in DB
            await _channel.Writer.WriteAsync(new AnalysisRequest(jobId, null, AnalysisRequestType.Resume));
            return Accepted(new { status = "Queued", message = "Resume generation started." });
        }

        // POST: api/Ai/generate-cover-letter/{jobId}
        [HttpPost("generate-cover-letter/{jobId}")]
        public async Task<IActionResult> GenerateCoverLetter(int jobId)
        {
            await _channel.Writer.WriteAsync(new AnalysisRequest(jobId, null, AnalysisRequestType.CoverLetter));
            return Accepted(new { status = "Queued", message = "Cover letter generation started." });
        }
    }
}