using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("demo")]
        public IActionResult GetDemoStatus()
        {
            var demoSection = _configuration.GetSection("DemoMode");
            
            return Ok(new
            {
                IsEnabled = demoSection.GetValue<bool>("Enabled"),
                CleanupIntervalMinutes = demoSection.GetValue<int>("CleanupIntervalMinutes")
            });
        }
    }
}
