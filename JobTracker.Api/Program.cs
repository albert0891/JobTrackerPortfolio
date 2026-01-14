using JobTracker.Api.Data;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Channels;

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

// Register SignalR
builder.Services.AddSignalR();

// Register Channel for Background Queue (Unbounded for simplicity, can use Bounded for backpressure)
builder.Services.AddSingleton(Channel.CreateUnbounded<AnalysisRequest>());

// Register Background Service
builder.Services.AddHostedService<BackgroundAnalysisService>();

// Register CORS Policy
// SignalR requires AllowCredentials() and specific origins (cannot use AllowAnyOrigin with Credentials)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins(allowedOrigins) // Read from appsettings.json
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
});

// Register Swagger (OpenAPI) Services for CONTROLLERS
// This service provides the metadata (routes, parameters) needed for Swagger/OpenAPI generation.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // This provides standard information for the Swagger UI page
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobTracker.Api", Version = "v1" });
});

// Register Services
builder.Services.AddScoped<IJobService, JobService>();
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


// Map SignalR Hub
app.MapHub<AnalysisHub>("/analysisHub");

// Run the application
app.Run();