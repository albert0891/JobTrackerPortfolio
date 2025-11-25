// Import all necessary namespaces
using JobTracker.Api.Data;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Import this for full Swagger configuration

var builder = WebApplication.CreateBuilder(args);

// --- Dependency Injection (DI) Container ---

// 1. Register Controllers
// This tells the app to find and use our Controller classes (like JobApplicationsController).
builder.Services.AddControllers();

// 2. Register Database Context
// This registers our ApplicationDbContext and configures it to use PostgreSQL
// with the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 3. Register CORS Policy
// This defines a security policy to allow our Angular app (at localhost:4200)
// to send requests to this API.
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 4. Register Swagger (OpenAPI) Services for CONTROLLERS
// This replaces your 'AddOpenApi()'
// 'AddSwaggerGen' is the correct service for finding Controller-based API endpoints.
builder.Services.AddSwaggerGen(c =>
{
    // This provides standard information for the Swagger UI page
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobTracker.Api", Version = "v1" });
});

// 5. Register the concrete Service against its Interface for Dependency Injection
builder.Services.AddScoped<IJobService, JobService>();

// --- End of DI Container ---

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---
// This defines the 'middleware' that handles every HTTP request.
// Order matters here!

// 1. Configure for Development environments
if (app.Environment.IsDevelopment())
{
    // This enables the interactive Swagger UI documentation page
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // This tells the UI where to find the generated documentation file
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobTracker.Api v1");
        // We set the route to the root for convenience
        c.RoutePrefix = string.Empty;
    });
}

// 2. Enable HTTPS Redirection
// This automatically redirects any HTTP request to its HTTPS equivalent.
app.UseHttpsRedirection();

// 3. Enable CORS
// This applies the CORS policy we defined above, allowing our Angular app to connect.
app.UseCors(myAllowSpecificOrigins);

// 4. Enable Authorization (we'll use this later for security)
app.UseAuthorization();

// 5. Map Controllers
// This tells the app to map incoming requests to the routes defined
// on our Controller classes (e.g., [Route("api/[controller]")])
app.MapControllers();

// 6. Run the application
app.Run();