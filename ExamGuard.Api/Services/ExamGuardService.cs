using System.Collections.Concurrent;
using ExamGuard.Api.DTOs;
using ExamGuard.Api.Models;

namespace ExamGuard.Api.Services;

public class ExamGuardService : IExamGuardService
{
    private readonly ConcurrentDictionary<Guid, ExamGuardSession> _sessions = new();

    public CreateGuardSessionResponse CreateSession()
    {
        var session = new ExamGuardSession
        {
            SessionId = Guid.NewGuid(),
            StudentId = "student-demo-001",
            StudentName = "Nguyễn Văn An",
            ExamId = "exam-demo-001",
            ExamName = "Kiểm tra giữa kỳ - Mạng máy tính",
            Status = ExamGuardStatus.Waiting,
            Safe = null,
            RiskScore = 0,
            Message = "Chờ báo cáo từ công cụ giám sát."
        };

        _sessions[session.SessionId] = session;

        return new CreateGuardSessionResponse
        {
            SessionId = session.SessionId,
            StudentId = session.StudentId,
            StudentName = session.StudentName,
            ExamId = session.ExamId,
            Status = session.Status.ToString(),
            Safe = session.Safe,
            RiskScore = session.RiskScore,
            Message = session.Message,
            DetectedApps = session.DetectedApps.Select(app => new DetectedAppDto
            {
                Name = app.Name,
                Description = app.Description
            }).ToList(),
            CreatedAt = session.CreatedAt,
            LastCheckedAt = session.LastCheckedAt
        };
    }

    public GuardStatusResponse? GetStatus(Guid sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        return MapToStatusResponse(session);
    }

    public GuardStatusResponse? Report(Guid sessionId, GuardReportRequest request)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        session.Safe = request.Safe;
        session.RiskScore = request.RiskScore;
        session.Status = ParseStatus(request.Status);
        session.DetectedApps = request.DetectedApps?.Select(app => new DetectedAppDto
        {
            Name = app.Name,
            Description = app.Description
        }).ToList() ?? new List<DetectedAppDto>();
        session.MachineName = request.MachineName;
        session.Os = request.Os;
        session.LastCheckedAt = request.CheckedAt ?? DateTime.UtcNow;
        session.Message = string.IsNullOrWhiteSpace(request.Message) ? BuildMessage(session) : request.Message;

        return MapToStatusResponse(session);
    }

    public GuardStatusResponse? SimulateSafe(Guid sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        session.Status = ExamGuardStatus.Safe;
        session.Safe = true;
        session.RiskScore = 0;
        session.DetectedApps.Clear();
        session.Message = "Không phát hiện ứng dụng điều khiển từ xa.";
        session.LastCheckedAt = DateTime.UtcNow;

        return MapToStatusResponse(session);
    }

    public GuardStatusResponse? SimulateUnsafe(Guid sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        session.Status = ExamGuardStatus.Unsafe;
        session.Safe = false;
        session.RiskScore = 90;
        session.DetectedApps = new List<DetectedAppDto>
        {
            new() { Name = "AnyDesk.exe", Description = "Phát hiện ứng dụng điều khiển từ xa." }
        };
        session.Message = "Phát hiện ứng dụng điều khiển từ xa: AnyDesk.exe.";
        session.LastCheckedAt = DateTime.UtcNow;

        return MapToStatusResponse(session);
    }

    public GuardStatusResponse? Heartbeat(Guid sessionId, HeartbeatRequest request)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        session.LastHeartbeatAt = DateTime.UtcNow;
        session.Message = request.Message ?? "Heartbeat received.";

        return MapToStatusResponse(session);
    }

    private static GuardStatusResponse MapToStatusResponse(ExamGuardSession session)
    {
        return new GuardStatusResponse
        {
            SessionId = session.SessionId,
            Status = session.Status.ToString(),
            Safe = session.Safe,
            RiskScore = session.RiskScore,
            Message = session.Message,
            DetectedApps = session.DetectedApps.Select(app => new DetectedAppDto
            {
                Name = app.Name,
                Description = app.Description
            }).ToList(),
            LastCheckedAt = session.LastCheckedAt
        };
    }

    private static string BuildMessage(ExamGuardSession session)
    {
        return session.Status switch
        {
            ExamGuardStatus.Safe => "Môi trường an toàn để làm bài.",
            ExamGuardStatus.Unsafe => "Phát hiện ứng dụng điều khiển từ xa.",
            ExamGuardStatus.Checking => "Đang kiểm tra môi trường thi.",
            _ => "Chờ báo cáo từ công cụ giám sát."
        };
    }

    private static ExamGuardStatus ParseStatus(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "checking" => ExamGuardStatus.Checking,
            "safe" => ExamGuardStatus.Safe,
            "unsafe" => ExamGuardStatus.Unsafe,
            "tooloffline" => ExamGuardStatus.ToolOffline,
            "expired" => ExamGuardStatus.Expired,
            _ => ExamGuardStatus.Waiting
        };
    }
}
