using System.Diagnostics;
using ExamGuard.Agent.Models;

namespace ExamGuard.Agent.Services;

public class ProcessScanner
{
    private readonly HashSet<string> _suspiciousAppProcessNames;
    private readonly HashSet<string> _suspiciousServiceProcessNames;

    public ProcessScanner()
    {
        var remoteAppList = new RemoteAppListLoader().Load();
        _suspiciousAppProcessNames = remoteAppList.AppProcessNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        _suspiciousServiceProcessNames = remoteAppList.ServiceProcessNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public CheckResult Scan()
    {
        var processes = Process.GetProcesses();
        var detectedApps = new List<DetectedApp>();

        foreach (var process in processes)
        {
            try
            {
                var processName = process.ProcessName;
                if (_suspiciousAppProcessNames.Contains(processName))
                {
                    detectedApps.Add(new DetectedApp
                    {
                        Name = processName,
                        Description = "Phát hiện ứng dụng điều khiển từ xa đang chạy"
                    });
                }
                else if (_suspiciousServiceProcessNames.Contains(processName))
                {
                    detectedApps.Add(new DetectedApp
                    {
                        Name = processName,
                        Description = "Phát hiện dịch vụ điều khiển từ xa chạy nền"
                    });
                }
            }
            catch
            {
                // Ignore inaccessible process details.
            }
            finally
            {
                process.Dispose();
            }
        }

        if (detectedApps.Count == 0)
        {
            return new CheckResult
            {
                Safe = true,
                Status = "Safe",
                RiskScore = 0,
                DetectedApps = new List<DetectedApp>()
            };
        }

        return new CheckResult
        {
            Safe = false,
            Status = "Unsafe",
            RiskScore = 90,
            DetectedApps = detectedApps.DistinctBy(app => app.Name).ToList()
        };
    }
}
