using ExamGuard.Api.DTOs;
using ExamGuard.Api.Services;

namespace ExamGuard.Api.Tests;

public class ExamGuardServiceTests
{
    [Fact]
    public void CreateSession_ReturnsDefaultWaitingSession()
    {
        var service = new ExamGuardService();

        var session = service.CreateSession();

        Assert.NotEqual(Guid.Empty, session.SessionId);
        Assert.Equal("Waiting", session.Status);
        Assert.Equal("student-demo-001", session.StudentId);
        Assert.Null(session.Safe);
        Assert.Equal(0, session.RiskScore);
        Assert.Equal("Chờ báo cáo từ công cụ giám sát.", session.Message);
    }

    [Fact]
    public void SimulateUnsafe_ReportsAnyDeskDetection()
    {
        var service = new ExamGuardService();
        var created = service.CreateSession();

        var status = service.SimulateUnsafe(created.SessionId);

        Assert.NotNull(status);
        Assert.Equal("Unsafe", status!.Status);
        Assert.False(status.Safe);
        Assert.Contains(status.DetectedApps, app => app.Name == "AnyDesk.exe");
    }
}
