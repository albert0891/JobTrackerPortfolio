using JobTracker.Api.Models;

namespace JobTracker.Api.Data
{
    public static class SeedDataHelper
    {
        public static List<JobApplication> GetSeedJobs()
        {
            return new List<JobApplication>
            {
                new JobApplication
                {
                    JobTitle = "Frontend Engineer",
                    CompanyName = "Google",
                    DateApplied = new DateTime(2025, 11, 25, 10, 0, 0, DateTimeKind.Utc),
                    Status = "Applied",
                    JobDescription = "We are looking for an Angular expert...",
                    AiAnalysisResult = null,
                    GeneratedCoverLetter = null
                },
                new JobApplication
                {
                    JobTitle = ".NET Backend Developer",
                    CompanyName = "Microsoft",
                    DateApplied = new DateTime(2025, 11, 20, 14, 30, 0, DateTimeKind.Utc),
                    Status = "Interviewing",
                    JobDescription = "Experience with C# and Azure required...",
                    AiAnalysisResult = null,
                    GeneratedCoverLetter = null
                }
            };
        }
    }
}
