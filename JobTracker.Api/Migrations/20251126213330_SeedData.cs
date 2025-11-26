using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JobApplications",
                columns: new[] { "Id", "AiAnalysisResult", "CompanyName", "DateApplied", "GeneratedCoverLetter", "JobDescription", "JobTitle", "Status" },
                values: new object[,]
                {
                    { 1, null, "Google", new DateTime(2025, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc), null, "We are looking for an Angular expert...", "Frontend Engineer", "Applied" },
                    { 2, null, "Microsoft", new DateTime(2025, 11, 20, 14, 30, 0, 0, DateTimeKind.Utc), null, "Experience with C# and Azure required...", ".NET Backend Developer", "Interviewing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JobApplications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JobApplications",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
