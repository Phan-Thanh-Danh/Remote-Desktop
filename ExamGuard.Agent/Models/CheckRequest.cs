namespace ExamGuard.Agent.Models;

public class CheckRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
}
