namespace ExamGuard.Api.DTOs;

public class CreateGuardSessionResponse
{
    public Guid SessionId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool? Safe { get; set; }
    public int RiskScore { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<DetectedAppDto> DetectedApps { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}
