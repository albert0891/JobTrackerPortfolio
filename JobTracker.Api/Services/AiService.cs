using System.Text;
using System.Text.Json;
using JobTracker.Api.Data;
using Mscc.GenerativeAI;
using UglyToad.PdfPig;

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
            _model = _googleAI.GenerativeModel("gemini-2.5-flash");
        }

        public async Task<AiAnalysisDto?> AnalyzeJobAsync(int jobId, Stream resumeStream)
        {
            var jobApplication = await _context.JobApplications.FindAsync(jobId);
            if (jobApplication == null) return null;

            // 1. Extract text from the PDF stream using PdfPig
            string resumeText = ExtractTextFromPdf(resumeStream);

            if (string.IsNullOrWhiteSpace(resumeText))
            {
                throw new Exception("Could not read text from the uploaded PDF.");
            }

            // 2. Construct Prompt (Use the extracted text)
            string prompt = $@"
                You are an expert career coach. Analyze the following resume against the job description.
                Respond ONLY with a valid JSON object.
                
                JSON Schema:
                {{
                    ""matchScore"": integer (0-100),
                    ""strengths"": ""string"",
                    ""keywordsToIntegrate"": [""string"", ""string""]
                }}

                --- RESUME START ---
                {resumeText}
                --- RESUME END ---

                --- JOB DESCRIPTION START ---
                {jobApplication.JobDescription}
                --- JOB DESCRIPTION END ---
            ";

            try
            {
                var response = await _model.GenerateContent(prompt);
                string responseText = response.Text;

                if (responseText.StartsWith("```"))
                {
                    responseText = responseText.Replace("```json", "").Replace("```", "").Trim();
                }

                var analysis = JsonSerializer.Deserialize<AiAnalysisDto>(responseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Update DB with result
                jobApplication.AiAnalysisResult = responseText;
                await _context.SaveChangesAsync();

                return analysis;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Error: {ex.Message}");
                return null;
            }
        }

        // Helper method to parse PDF
        private string ExtractTextFromPdf(Stream pdfStream)
        {
            try
            {
                using var document = PdfDocument.Open(pdfStream);
                var sb = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    sb.Append(page.Text);
                    sb.Append(" ");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF Parsing Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}