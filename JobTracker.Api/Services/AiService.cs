using System.Text;
using System.Text.Json;
using JobTracker.Api.Data;
using JobTracker.Api.Models;
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

            // Save extracted text to DB
            jobApplication.ResumeText = resumeText;
            await _context.SaveChangesAsync();

            return await PerformAnalysisAsync(jobApplication, resumeText);
        }

        private async Task<AiAnalysisDto?> PerformAnalysisAsync(JobApplication job, string resumeText)
        {
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
                {job.JobDescription}
                --- JOB DESCRIPTION END ---
            ";

            try
            {
                var response = await _model.GenerateContent(prompt);
                string responseText = CleanResponse(response.Text);

                var analysis = JsonSerializer.Deserialize<AiAnalysisDto>(responseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Update DB with result
                job.AiAnalysisResult = responseText;
                await _context.SaveChangesAsync();

                return analysis;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GenerateTailoredResumeAsync(int jobId)
        {
            var job = await _context.JobApplications.FindAsync(jobId);
            if (job == null) return null;
            if (string.IsNullOrWhiteSpace(job.ResumeText)) throw new InvalidOperationException("Resume text is missing. Please re-upload your resume.");

            string prompt = $@"
                You are a professional resume writer. Rewrite the Candidate's Resume to better match the Job Description.
                Highlight skills and experiences relevant to the JD.
                Maintain the original Markdown structure of the resume if possible, or use a clean Markdown format.
                Do NOT invent false information. Only rephrase existing experience.
                
                Output the result in Markdown format.

                --- ORIGINAL RESUME ---
                {job.ResumeText}
                --- END ORIGINAL RESUME ---

                --- JOB DESCRIPTION ---
                {job.JobDescription}
                --- END JOB DESCRIPTION ---
            ";

            try
            {
                var response = await _model.GenerateContent(prompt);
                string result = CleanResponse(response.Text, removeCodeBlocks: true); // Remove markdown code blocks if AI wraps it

                job.GeneratedResume = result;
                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GenerateCoverLetterAsync(int jobId)
        {
            var job = await _context.JobApplications.FindAsync(jobId);
            if (job == null) return null;
            if (string.IsNullOrWhiteSpace(job.ResumeText)) throw new InvalidOperationException("Resume text is missing. Please re-upload your resume.");

            string prompt = $@"
                You are a professional career coach. Write a compelling Cover Letter for the Candidate applying to {job.CompanyName}.
                
                **Constraints:**
                1. Length: Approximately 250 words. Be concise and impactful.
                2. Tone: Professional, enthusiastic, and confident.
                
                **Structure:**
                1. **The Hook**: Start with a strong opening that grabs attention and states why you are writing.
                2. **The Meat**: Highlight 1-2 key achievements from the Resume that directly relate to the Job Description. Show, don't just tell.
                3. **The Fit**: Explain why you are a perfect cultural and technical fit for this specific company/role.
                4. **Call to Action**: End with a confident request for an interview (do NOT put section headers like 'The Hook' in the final output, just use the structure).
                
                Output in Markdown format.

                --- RESUME ---
                {job.ResumeText}
                --- END RESUME ---

                --- JOB DESCRIPTION ---
                {job.JobDescription}
                --- END JOB DESCRIPTION ---
            ";

            try
            {
                var response = await _model.GenerateContent(prompt);
                string result = CleanResponse(response.Text, removeCodeBlocks: true);

                job.GeneratedCoverLetter = result;
                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini Error: {ex.Message}");
                return null;
            }
        }

        // Helper helper to clean markdown code blocks
        private string CleanResponse(string text, bool removeCodeBlocks = false)
        {
            if (text.StartsWith("```"))
            {
                // Remove start ```json or ```markdown
                var firstLineEnd = text.IndexOf('\n');
                if (firstLineEnd > -1)
                {
                    text = text.Substring(firstLineEnd + 1);
                }
                // Remove end ```
                if (text.EndsWith("```"))
                {
                    text = text.Substring(0, text.Length - 3);
                }
            }
            return text.Trim();
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