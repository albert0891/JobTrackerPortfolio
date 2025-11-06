// This namespace follows your project's structure.
// It helps organize your code and prevents name conflicts.
namespace JobTracker.Api.Models
{
    public class JobApplication
    {
        // 'Id' is the Primary Key (PK) by convention.
        // EF Core will automatically make this an auto-incrementing integer.
        public int Id { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        // We use DateTime to store the date. 
        // We'll use UTC (Coordinated Universal Time) as a best practice.
        public DateTime DateApplied { get; set; }

        // This will store "Applied", "Interviewing", "Offer", "Rejected"
        public string Status { get; set; } = "Applied";

        // This will store the full job description text for AI analysis
        public string JobDescription { get; set; } = string.Empty;

        // These fields are 'nullable' (can be null) 
        // because they won't have data until the AI runs.
        public string? AiAnalysisResult { get; set; }
        public string? GeneratedCoverLetter { get; set; }
    }
}