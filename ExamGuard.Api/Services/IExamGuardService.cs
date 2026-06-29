using ExamGuard.Api.DTOs;

namespace ExamGuard.Api.Services;

public interface IExamGuardService
{
    CreateGuardSessionResponse CreateSession();
    GuardStatusResponse? GetStatus(Guid sessionId);
    GuardStatusResponse? Report(Guid sessionId, GuardReportRequest request);
    GuardStatusResponse? SimulateSafe(Guid sessionId);
    GuardStatusResponse? SimulateUnsafe(Guid sessionId);
    GuardStatusResponse? Heartbeat(Guid sessionId, HeartbeatRequest request);
}
