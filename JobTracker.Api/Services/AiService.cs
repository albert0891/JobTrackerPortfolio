using System.Text.Json;
using JobTracker.Api.Data;
using JobTracker.Api.Models;
using Mscc.GenerativeAI;
using Microsoft.Extensions.Configuration;

namespace JobTracker.Api.Services
{
    // Simple DTO for the AI response
    public class AiAnalysisDto
    {
        public int MatchScore { get; set; }
        public string Strengths { get; set; } = string.Empty;
        public string[] KeywordsToIntegrate { get; set; } = [];
    }

    public class AiService
    {
        private readonly ApplicationDbContext _context;
        private readonly GoogleAI _googleAI;
        private readonly GenerativeModel _model;

        public AiService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

            var apiKey = configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini:ApiKey is not configured.");
            }

            // Initialize the Google AI client
            _googleAI = new GoogleAI(apiKey);

            // Use the string name to avoid Enum issues in different versions
            // "gemini-1.5-flash" is the current fast model
            _model = _googleAI.GenerativeModel("gemini-1.5-flash");
        }

        public async Task<AiAnalysisDto?> AnalyzeJobAsync(int jobId)
        {
            var jobApplication = await _context.JobApplications.FindAsync(jobId);
            if (jobApplication == null) return null;

            // --- Hardcoded for MVP ---
            // Meaning: We are faking the resume upload feature for now 
            // by using a static string so we can test the AI logic immediately.
            string myResume = @"
                Albert, Full-Stack Engineer.
                Skills: Angular, .NET Web API, C#, TypeScript, Docker, SQL Server.
                Experience: Built a Kanban board application with drag-and-drop features.
            ";
            // -------------------------

            string prompt = $@"
                You are an expert career coach. Analyze the following resume against the job description.
                Respond ONLY with a valid JSON object.
                
                JSON Schema:
                {{
                    ""matchScore"": integer (0-100),
                    ""strengths"": ""string"",
                    ""keywordsToIntegrate"": [""string"", ""string""]
                }}

                RESUME: {myResume}
                JOB DESCRIPTION: {jobApplication.JobDescription}
            ";

            try
            {
                var response = await _model.GenerateContent(prompt);

                // Usually the text is in response.Text
                string responseText = response.Text;

                // Cleanup markdown if Gemini puts ```json ... ```
                if (responseText.StartsWith("```"))
                {
                    responseText = responseText.Replace("```json", "").Replace("```", "").Trim();
                }

                var analysis = JsonSerializer.Deserialize<AiAnalysisDto>(responseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Update DB
                jobApplication.AiAnalysisResult = responseText;
                await _context.SaveChangesAsync();

                return analysis;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Error: {ex.Message}");
                // In production, use ILogger to log this
                return null;
            }
        }
    }
}