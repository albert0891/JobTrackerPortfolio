using JobTracker.Api.Data;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// --- Dependency Injection (DI) Container ---

// Register Controllers
// This tells the app to find and use our Controller classes (like JobApplicationsController).
builder.Services.AddControllers();

// Register Database Context
// This registers our ApplicationDbContext and configures it to use PostgreSQL
// with the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register CORS Policy
// This defines a security policy to allow our Angular app (at localhost:4200)
// to send requests to this API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Register Swagger (OpenAPI) Services for CONTROLLERS
// This service provides the metadata (routes, parameters) needed for Swagger/OpenAPI generation.
builder.Services.AddEndpointsApiExplorer();
// 'AddSwaggerGen' is the correct service for finding Controller-based API endpoints.
builder.Services.AddSwaggerGen(c =>
{
    // This provides standard information for the Swagger UI page
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobTracker.Api", Version = "v1" });
});

// Register the concrete Service against its Interface for Dependency Injection
builder.Services.AddScoped<IJobService, JobService>();

// Register the AiService for Dependency Injection
builder.Services.AddScoped<AiService>();

// Add Rate Limiter Service
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Define a policy named "GeminiPolicy"
    options.AddFixedWindowLimiter("GeminiPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // Time window: 1 minute
        opt.PermitLimit = 5;                  // Permit limit: 5 requests
        opt.QueueLimit = 0;                   // Queue limit: 0 (reject immediately if exceeded)
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

// --- End of DI Container ---

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---
// This defines the 'middleware' that handles every HTTP request.
// Order matters here!

// This enables the interactive Swagger UI documentation page
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // This tells the UI where to find the generated documentation file
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobTracker.Api v1");
    // We set the route to the root for convenience
    c.RoutePrefix = string.Empty;
});

// Enable HTTPS Redirection
// This automatically redirects any HTTP request to its HTTPS equivalent.
//app.UseHttpsRedirection();

// Enable CORS
// This applies the CORS policy we defined above, allowing our Angular app to connect.
app.UseCors("AllowAll");

// Enable Rate Limiting
// Must be after UseCors so that 429 responses contain CORS headers.
app.UseRateLimiter();

// Enable Authorization (we'll use this later for security)
app.UseAuthorization();

// Map Controllers
// This tells the app to map incoming requests to the routes defined
// on our Controller classes (e.g., [Route("api/[controller]")])
app.MapControllers();

// Run the application
app.Run();