using ExamGuard.Api.DTOs;
using ExamGuard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamGuard.Api.Controllers;

[ApiController]
[Route("api/exam-guard")]
public class ExamGuardController : ControllerBase
{
    private readonly IExamGuardService _examGuardService;

    public ExamGuardController(IExamGuardService examGuardService)
    {
        _examGuardService = examGuardService;
    }

    [HttpPost("sessions")]
    public ActionResult<CreateGuardSessionResponse> CreateSession()
    {
        var session = _examGuardService.CreateSession();
        return Ok(session);
    }

    [HttpGet("sessions/{sessionId}/status")]
    public ActionResult<GuardStatusResponse> GetStatus(Guid sessionId)
    {
        var status = _examGuardService.GetStatus(sessionId);
        if (status is null)
        {
            return NotFound(new { message = "Session not found." });
        }

        return Ok(status);
    }

    [HttpPost("sessions/{sessionId}/report")]
    public ActionResult<GuardStatusResponse> Report(Guid sessionId, [FromBody] GuardReportRequest request)
    {
        var status = _examGuardService.Report(sessionId, request);
        if (status is null)
        {
            return NotFound(new { message = "Session not found." });
        }

        return Ok(status);
    }

    [HttpPost("sessions/{sessionId}/simulate-safe")]
    public ActionResult<GuardStatusResponse> SimulateSafe(Guid sessionId)
    {
        var status = _examGuardService.SimulateSafe(sessionId);
        if (status is null)
        {
            return NotFound(new { message = "Session not found." });
        }

        return Ok(status);
    }

    [HttpPost("sessions/{sessionId}/simulate-unsafe")]
    public ActionResult<GuardStatusResponse> SimulateUnsafe(Guid sessionId)
    {
        var status = _examGuardService.SimulateUnsafe(sessionId);
        if (status is null)
        {
            return NotFound(new { message = "Session not found." });
        }

        return Ok(status);
    }

    [HttpPost("sessions/{sessionId}/heartbeat")]
    public ActionResult<GuardStatusResponse> Heartbeat(Guid sessionId, [FromBody] HeartbeatRequest request)
    {
        var status = _examGuardService.Heartbeat(sessionId, request);
        if (status is null)
        {
            return NotFound(new { message = "Session not found." });
        }

        return Ok(status);
    }
}
