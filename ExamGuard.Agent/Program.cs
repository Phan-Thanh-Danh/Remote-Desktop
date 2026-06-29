using System.Text;
using System.Text.Json;
using ExamGuard.Agent.Models;
using ExamGuard.Agent.Services;

var processScanner = new ProcessScanner();
var backendReporter = new BackendReporter();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteCors", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return (uri.Host == "localhost" || uri.Host == "127.0.0.1")
                    && uri.Port >= 5173
                    && uri.Port <= 5199;
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.Urls.Clear();
app.Urls.Add("http://127.0.0.1:17891");
app.UseCors("ViteCors");

app.MapGet("/health", () => Results.Json(new
{
    running = true,
    name = "ExamGuard.Agent",
    version = "1.0.0"
}));

app.MapPost("/check", async (HttpContext context) =>
{
    try
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var request = JsonSerializer.Deserialize<CheckRequest>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (request is null || string.IsNullOrWhiteSpace(request.SessionId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "SessionId is required." });
            return;
        }

        Console.WriteLine($"[check] received sessionId={request.SessionId} apiBaseUrl={request.ApiBaseUrl}");

        var result = processScanner.Scan();
        if (result.DetectedApps.Count > 0)
        {
            Console.WriteLine($"[check] detected {result.DetectedApps.Count} remote-control process(es)");
            foreach (var app in result.DetectedApps)
            {
                Console.WriteLine($"  - {app.Name}");
            }
        }
        else
        {
            Console.WriteLine("[check] no suspicious remote-control process found");
        }

        result.Message = result.Safe
            ? "Không phát hiện ứng dụng điều khiển từ xa."
            : "Phát hiện ứng dụng điều khiển từ xa.";

        var reportSucceeded = false;
        if (!string.IsNullOrWhiteSpace(request.ApiBaseUrl))
        {
            reportSucceeded = await backendReporter.ReportAsync(request.ApiBaseUrl, request.SessionId, result);
        }

        var response = new
        {
            success = true,
            sessionId = request.SessionId,
            safe = result.Safe,
            status = result.Status,
            riskScore = result.RiskScore,
            message = result.Message,
            detectedApps = result.DetectedApps,
            backendReported = reportSucceeded
        };

        await context.Response.WriteAsJsonAsync(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[check] error: {ex.Message}");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { success = false, error = ex.Message });
    }
});

Console.WriteLine("[agent] starting ExamGuard.Agent on http://127.0.0.1:17891");
app.Run();
