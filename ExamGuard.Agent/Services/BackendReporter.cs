using System.Net.Http.Json;
using ExamGuard.Agent.Models;

namespace ExamGuard.Agent.Services;

public class BackendReporter
{
    private readonly HttpClient _httpClient;

    public BackendReporter(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<bool> ReportAsync(string apiBaseUrl, string sessionId, CheckResult result)
    {
        try
        {
            var endpoint = $"{apiBaseUrl.TrimEnd('/')}/api/exam-guard/sessions/{sessionId}/report";
            var payload = new
            {
                safe = result.Safe,
                riskScore = result.RiskScore,
                status = result.Status,
                message = result.Message,
                detectedApps = result.DetectedApps,
                machineName = Environment.MachineName,
                os = Environment.OSVersion.VersionString,
                checkedAt = DateTime.UtcNow
            };

            Console.WriteLine($"[report] posting to {endpoint}");
            var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[report] status={response.StatusCode} body={content}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[report] failed: {ex.Message}");
            return false;
        }
    }
}
