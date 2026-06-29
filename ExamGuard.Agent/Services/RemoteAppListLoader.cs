using System.Data;
using System.Text;
using ExcelDataReader;

namespace ExamGuard.Agent.Services;

public class RemoteAppListLoader
{
    private static readonly string[] DefaultSuspiciousAppProcessNames =
    {
        // Remote Support - msra
        "msra",
        "RAS",
        
        // Remote Support - UltraViewer
        "UltraViewer",
        "UltraViewer_Desktop",
        
        // Remote Support - TeamViewer
        "TeamViewer",
        "tv_w32",
        "tv_w64",
        
        // Remote Support - AnyDesk
        "AnyDesk",
        
        // Remote Desktop - RustDesk
        "RustDesk",
        
        // Remote Desktop - Microsoft RDP
        "mstsc",
        
        // VNC - UltraVNC
        "UltraVNC",
        "ultravnc",
        
        // VNC - VNC server/viewer
        "vncviewer",
        "vnc",
        
        // VNC - TightVNC
        "TightVNC",
        "tvnviewer",
        "tightvnc",
        
        // VNC - RealVNC
        "RealVNC",
        "realvnc",
        
        // Remote Support - Splashtop
        "Splashtop",
        "SplashApp",
        "SplashUpright",
        
        // Remote Support - LogMeIn
        "LogMeIn",
        "LogMgLiftRt",
        
        // Remote Support - Ammyy
        "Ammyy",
        "AA-x86",
        "AnyViewer",
        "aa-admin",
        
        // Remote Support - GoToAssist
        "GoToAssist",
        "g2ax",
        "GoAssist",
        
        // Remote Support - ISL
        "ISL",
        "ISLlight",
        "ISL_anywhere",
        
        // Remote Support - Supremo
        "Supremo",
        "supremo",
        
        // Remote Support - AeroAdmin
        "AeroAdmin",
        "aeroadmin",
        
        // Remote Support - TeamConnect
        "TeamConnect",
        
        // Remote Support - Raimon
        "Raimon",
        "raimon",
        
        // Remote Desktop - Parsec
        "Parsec",
        "parsec",
        
        // Remote/Streaming - MeshCentral Agent
        "MeshAgent",
        "MeshCentral",
        
        // Remote Desktop - DeskConnect
        "DeskConnect",
        "deskconnect",
        
        // Remote Support - Harmon
        "Harmon",
        "remote_utilities",
        
        // Remote Support - Guacamole
        "Guacamole",
        "guac-client",
        "guacamole",
        
        // Remote Support - BroadCast
        "BroadCast",
        "broadcast",
        
        // Remote Admin - Domeque
        "Domeque",
        "DwRC",
        "DwRCS3T",
        "DWMRC",
        
        // Remote Admin - Dameware
        "Dameware",
        "DWHRConfig",
        "MiniHelper",
        
        // Network/System - NetSupport
        "NetSupport",
        "NSupport",
        "NSM",
        "RAdmin",
        
        // Classroom Control - NETSupport School
        "NETSupport",
        "NETSupport.School",
        
        // RBM - Kaseya
        "Kaseya",
        "Kaseya3",
        
        // RBM - NinjaOne
        "NinjaOne",
        "Ninjaone",
        
        // RBM - Altris
        "Altris",
        "AltrisAgent",
        "SpotlightStreamer",
        
        // RBM - Secure Access
        "Secure.Spotlight",
        
        // Remote Support - Logitech Rescue
        "Logitech",
        "Rescue",
        "LMiRescue",
        "LogitechRescue",
        
        // Remote Access - LiveUpdate
        "LiveUpdate",
        "LiveUpdatesec-Manager",
        
        // Remote Connection - mInternetDC
        "mInternetDC",
        "mInternetG",
        
        // Remote Connection - Royal TS
        "RoyalTS",
        "RoyalTSX",
        "RoyalServer",
        
        // Manager - RoboRH
        "RoboRH",
        
        // Remote/Linux - Remmina
        "Remmina",
        "remmina",
        
        // Remote/Linux - XRDP
        "xrdp",
        "xfreerdp",
        "freerdp",
        
        // VDI - VDI/Workspace
        "VDI.Workspace",
        "PCoIP",
        "Citrix",
        "VMware",
        "Horizon",
        
        // Remote Graphics - HP Airyseem
        "HP_Airyseem",
        "Teradici",
        "PCoIP.Client",
        
        // Chrome Remote Desktop
        "ChromeRemoteDesktop",
        "chromeremotedesktop",
        
        // NoMachine
        "NoMachine",
        "nxplayer"
    };

    private static readonly string[] DefaultSuspiciousServiceProcessNames =
    {
        // Services that can keep a machine remotely reachable without the main UI.
        "TeamViewer",
        "TeamViewer_Service",
        "TeamViewService",
        "AnyDesk",
        "RustDesk",
        "remoting_host",
        "chromoting",
        "tvnserver",
        "uvnc_service",
        "winvnc",
        "vncserver",
        "SplashtopRemoteService",
        "LogMeIn",
        "RServer3"
    };

    public IReadOnlyList<string> LoadNames()
    {
        var list = Load();
        return list.AppProcessNames
            .Concat(list.ServiceProcessNames)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public RemoteAppList Load()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var serviceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var defaultName in DefaultSuspiciousAppProcessNames)
        {
            var normalized = NormalizeProcessName(defaultName);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                names.Add(normalized);
            }
        }

        foreach (var defaultName in DefaultSuspiciousServiceProcessNames)
        {
            var normalized = NormalizeProcessName(defaultName);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                serviceNames.Add(normalized);
            }
        }

        foreach (var candidatePath in GetCandidatePaths())
        {
            if (!File.Exists(candidatePath))
            {
                continue;
            }

            try
            {
                foreach (var name in ReadNamesFromExcel(candidatePath))
                {
                    var normalized = NormalizeProcessName(name);
                    if (!string.IsNullOrWhiteSpace(normalized))
                    {
                        names.Add(normalized);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[app-list] failed to read {candidatePath}: {ex.Message}");
            }
        }

        return new RemoteAppList(
            names.ToList(),
            serviceNames.ToList());
    }

    private static IEnumerable<string> GetCandidatePaths()
    {
        var candidates = new List<string>();

        var configuredPath = Environment.GetEnvironmentVariable("EXAMGUARD_REMOTE_APPS_FILE");
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            candidates.Add(configuredPath);
        }

        candidates.AddRange(new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "remote-control-apps.xlsx"),
            Path.Combine(Directory.GetCurrentDirectory(), "remote-control-apps.xls"),
            Path.Combine(AppContext.BaseDirectory, "remote-control-apps.xlsx"),
            Path.Combine(AppContext.BaseDirectory, "remote-control-apps.xls")
        });

        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        if (currentDirectory.Exists)
        {
            foreach (var file in currentDirectory.GetFiles("*.xlsx", SearchOption.AllDirectories)
                         .Concat(currentDirectory.GetFiles("*.xls", SearchOption.AllDirectories)))
            {
                if (!candidates.Contains(file.FullName, StringComparer.OrdinalIgnoreCase))
                {
                    candidates.Add(file.FullName);
                }
            }
        }

        return candidates;
    }

    private static IEnumerable<string> ReadNamesFromExcel(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var result = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = false
            }
        });

        foreach (DataTable table in result.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                var value = row[0]?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value;
                }
            }
        }
    }

    private static string NormalizeProcessName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();

        if (trimmed.Contains("://", StringComparison.Ordinal))
        {
            trimmed = trimmed.Split("://", 2)[1];
        }

        var withoutExtension = Path.GetFileNameWithoutExtension(trimmed);
        return withoutExtension.Trim();
    }
}

public sealed record RemoteAppList(
    IReadOnlyList<string> AppProcessNames,
    IReadOnlyList<string> ServiceProcessNames);
