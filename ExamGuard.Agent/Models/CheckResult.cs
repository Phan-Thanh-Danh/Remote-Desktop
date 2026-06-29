namespace ExamGuard.Agent.Models;

public class CheckResult
{
    public bool Safe { get; set; }
    public string Status { get; set; } = "Safe";
    public int RiskScore { get; set; }
    public string Message { get; set; } = "Không phát hiện ứng dụng điều khiển từ xa.";
    public List<DetectedApp> DetectedApps { get; set; } = new();
}
