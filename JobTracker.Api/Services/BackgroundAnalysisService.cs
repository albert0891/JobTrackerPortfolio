using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;

namespace JobTracker.Api.Services;

public enum AnalysisRequestType { Analyze, Resume, CoverLetter }
public record AnalysisRequest(int JobId, Stream? ResumeStream, AnalysisRequestType Type);

public class BackgroundAnalysisService : BackgroundService
{
    private readonly Channel<AnalysisRequest> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<AnalysisHub> _hubContext;
    private readonly ILogger<BackgroundAnalysisService> _logger;

    public BackgroundAnalysisService(
        Channel<AnalysisRequest> channel,
        IServiceScopeFactory scopeFactory,
        IHubContext<AnalysisHub> hubContext,
        ILogger<BackgroundAnalysisService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Analysis Service started.");

        await foreach (var request in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessAnalysisAsync(request, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing analysis for JobId {JobId}", request.JobId);
                await _hubContext.Clients.All.SendAsync("JobStatusUpdate", request.JobId, "Failed", stoppingToken);
            }
        }
    }

    private async Task ProcessAnalysisAsync(AnalysisRequest request, CancellationToken token)
    {
        _logger.LogInformation("Processing {Type} for JobId {JobId}", request.Type, request.JobId);

        // Notify client: Processing
        await _hubContext.Clients.All.SendAsync("JobStatusUpdate", request.JobId, "Processing", request.Type.ToString(), token);

        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var aiService = scope.ServiceProvider.GetRequiredService<AiService>();

                try
                {
                    object? result = null;

                    switch (request.Type)
                    {
                        case AnalysisRequestType.Analyze:
                            if (request.ResumeStream == null) throw new ArgumentNullException(nameof(request.ResumeStream));
                            result = await aiService.AnalyzeJobAsync(request.JobId, request.ResumeStream);
                            break;
                        case AnalysisRequestType.Resume:
                            result = await aiService.GenerateTailoredResumeAsync(request.JobId);
                            break;
                        case AnalysisRequestType.CoverLetter:
                            result = await aiService.GenerateCoverLetterAsync(request.JobId);
                            break;
                    }

                    if (result != null)
                    {
                        await _hubContext.Clients.All.SendAsync("JobStatusUpdate", request.JobId, "Done", request.Type.ToString(), token);

                        // Send specific event based on type
                        if (request.Type == AnalysisRequestType.Analyze)
                            await _hubContext.Clients.All.SendAsync("AnalysisComplete", request.JobId, result, token);
                        else if (request.Type == AnalysisRequestType.Resume)
                            await _hubContext.Clients.All.SendAsync("ResumeGenerated", request.JobId, result, token);
                        else if (request.Type == AnalysisRequestType.CoverLetter)
                            await _hubContext.Clients.All.SendAsync("CoverLetterGenerated", request.JobId, result, token);
                    }
                    else
                    {
                        await _hubContext.Clients.All.SendAsync("JobStatusUpdate", request.JobId, "Failed", request.Type.ToString(), token);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing {Type}", request.Type);
                    await _hubContext.Clients.All.SendAsync("JobStatusUpdate", request.JobId, "Failed", request.Type.ToString(), token);

                    // Send specific error message
                    await _hubContext.Clients.All.SendAsync("JobError", request.JobId, ex.Message, token);
                }
            }
        }
        finally
        {
            // Dispose the stream if present, even if an error occurred above
            if (request.ResumeStream != null)
            {
                await request.ResumeStream.DisposeAsync();
            }
        }
    }
}
