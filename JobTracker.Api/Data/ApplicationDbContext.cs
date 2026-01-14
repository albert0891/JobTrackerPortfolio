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
            // Seed Data: Use the shared helper but assign IDs for EF Migrations
            var seedJobs = SeedDataHelper.GetSeedJobs();
            seedJobs[0].Id = 1;
            seedJobs[1].Id = 2;

            modelBuilder.Entity<JobApplication>().HasData(seedJobs);
        }
    }
}