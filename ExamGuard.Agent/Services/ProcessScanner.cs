using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
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
        var detectedApps = new List<DetectedApp>();

        // 1. Scan Processes
        var processes = Process.GetProcesses();
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
                        Description = "Phát hiện tiến trình dịch vụ điều khiển từ xa đang chạy"
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

        // 2. Scan Windows Services (only on Windows)
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                var services = ServiceController.GetServices();
                foreach (var service in services)
                {
                    try
                    {
                        var serviceName = service.ServiceName;
                        if (_suspiciousServiceProcessNames.Contains(serviceName) && service.Status == ServiceControllerStatus.Running)
                        {
                            detectedApps.Add(new DetectedApp
                            {
                                Name = serviceName,
                                Description = "Phát hiện dịch vụ điều khiển từ xa chạy nền"
                            });
                        }
                    }
                    catch
                    {
                        // Ignore inaccessible service details.
                    }
                    finally
                    {
                        service.Dispose();
                    }
                }
            }
            catch
            {
                // Ignore if we can't get services (e.g. permission issues)
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
