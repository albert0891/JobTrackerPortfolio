using Microsoft.AspNetCore.SignalR;

namespace JobTracker.Api.Services;

public class AnalysisHub : Hub
{
    // We can add methods here if the client needs to send messages to the server,
    // but for now, we only use it to push notifications from server to client.
}
