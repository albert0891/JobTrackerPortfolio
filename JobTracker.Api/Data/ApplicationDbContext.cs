using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Data
{
    // Our DbContext class inherits from the base DbContext provided by EF Core
    public class ApplicationDbContext : DbContext
    {
        // This is the constructor. It's needed for 'Dependency Injection'.
        // It takes 'options' (like the connection string) and passes them
        // to the base DbContext class.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // This DbSet represents a table in our database.
        // We are telling EF Core: "I want a table called 'JobApplications'
        // that is based on my 'JobApplication' model."
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Data: This will be inserted automatically when you run migration
            modelBuilder.Entity<JobApplication>().HasData(
                new JobApplication
                {
                    Id = 1, // Providing ID is crucial for idempotent seeding
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
                    Id = 2,
                    JobTitle = ".NET Backend Developer",
                    CompanyName = "Microsoft",
                    DateApplied = new DateTime(2025, 11, 20, 14, 30, 0, DateTimeKind.Utc),
                    Status = "Interviewing",
                    JobDescription = "Experience with C# and Azure required...",
                    AiAnalysisResult = null,
                    GeneratedCoverLetter = null
                }
            );
        }
    }
}