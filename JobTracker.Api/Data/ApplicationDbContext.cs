// We must import the namespaces we need
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

// This defines the namespace for our data-related classes
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
    }
}