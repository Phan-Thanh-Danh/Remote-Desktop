namespace ExamGuard.Api.DTOs;

public class GuardStatusResponse
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool? Safe { get; set; }
    public int RiskScore { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<DetectedAppDto> DetectedApps { get; set; } = new();
    public DateTime? LastCheckedAt { get; set; }
}
