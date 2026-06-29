using System.ComponentModel.DataAnnotations;

namespace ExamGuard.Api.DTOs;

public class GuardReportRequest
{
    [Required]
    public bool Safe { get; set; }

    [Range(0, 100)]
    public int RiskScore { get; set; }

    public string? Status { get; set; }

    public string? Message { get; set; }

    public List<DetectedAppDto>? DetectedApps { get; set; }

    public string? MachineName { get; set; }

    public string? Os { get; set; }

    public DateTime? CheckedAt { get; set; }
}
