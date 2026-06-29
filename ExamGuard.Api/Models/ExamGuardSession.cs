using ExamGuard.Api.DTOs;

namespace ExamGuard.Api.Models;

public class ExamGuardSession
{
    public Guid SessionId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public ExamGuardStatus Status { get; set; }
    public bool? Safe { get; set; }
    public int RiskScore { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<DetectedAppDto> DetectedApps { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }
    public DateTime? LastHeartbeatAt { get; set; }
    public string? MachineName { get; set; }
    public string? Os { get; set; }
}

public enum ExamGuardStatus
{
    Waiting,
    Checking,
    Safe,
    Unsafe,
    ToolOffline,
    Expired
}
