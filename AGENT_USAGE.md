# ExamGuard Agent - Hướng dẫn chạy

## ✅ Cách 1: Chạy từ Script (Dễ nhất)

### Windows PowerShell:
```powershell
cd d:\Remote Desktop
.\Run-ExamGuardAgent.ps1
```

### Windows Command Prompt (cmd):
```cmd
cd d:\Remote Desktop
Run-ExamGuardAgent.bat
```

## ✅ Cách 2: Chạy Executable trực tiếp

```powershell
d:\Remote Desktop\ExamGuard.Agent\bin\Release\net8.0\win-x64\publish\ExamGuard.Agent.exe
```

## ✅ Cách 3: Chạy từ Source Code

```powershell
cd "d:\Remote Desktop\ExamGuard.Agent"
dotnet run
```

---

## 📡 API Endpoint

Khi agent chạy, nó sẽ lắng nghe trên: **http://127.0.0.1:17891**

### 1. Health Check (Kiểm tra agent còn sống)
```powershell
Invoke-RestMethod -Uri "http://127.0.0.1:17891/health"
```

**Response:**
```json
{
  "running": true,
  "name": "ExamGuard.Agent",
  "version": "1.0.0"
}
```

### 2. Quét Ứng dụng Remote Control
```powershell
$body = @{
    sessionId = "44661636-e566-4514-a211-1488678ab2cd"
    apiBaseUrl = "http://127.0.0.1:5204"  # Backend ExamGuard API
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://127.0.0.1:17891/check" `
    -ContentType "application/json" -Body $body
```

**Response (nếu an toàn):**
```json
{
  "success": true,
  "sessionId": "44661636-e566-4514-a211-1488678ab2cd",
  "safe": true,
  "status": "Safe",
  "riskScore": 0,
  "message": "Không phát hiện ứng dụng điều khiển từ xa.",
  "detectedApps": [],
  "backendReported": true
}
```

**Response (nếu phát hiện rủi ro):**
```json
{
  "success": true,
  "sessionId": "44661636-e566-4514-a211-1488678ab2cd",
  "safe": false,
  "status": "Unsafe",
  "riskScore": 90,
  "message": "Phát hiện ứng dụng điều khiển từ xa.",
  "detectedApps": [
    {
      "name": "UltraViewer_Service",
      "description": "Phát hiện process điều khiển từ xa"
    }
  ],
  "backendReported": true
}
```

---

## 🎯 Danh sách ứng dụng được quét (60+ keyword)

- **Remote Support**: TeamViewer, AnyDesk, LogMeIn, GoToAssist, Supremo, AeroAdmin, Ammyy, AnyViewer, DeskConnect, ISL, BroadCast
- **Remote Desktop**: RustDesk, mstsc, TermService, ChromeRemoteDesktop
- **VNC**: UltraVNC, TightVNC, RealVNC, vncserver, vncviewer
- **Streaming**: Parsec, MeshAgent, Guacamole
- **Cloud/VDI**: PCoIP, Citrix, VMware, Horizon
- **Linux**: Remmina, xrdp, x2go, xpra
- **Khác**: Dameware, NetSupport, Kaseya, NinjaOne, Altris, RoyalTS, NoMachine, Splashtop

---

## 💡 Ghi chú

- Agent quét process đang chạy trên máy
- Nếu phát hiện ứng dụng remote, nó sẽ báo về backend (nếu cung cấp `apiBaseUrl`)
- SessionId phải là GUID hợp lệ (format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
- Nếu port 17891 đã được sử dụng, script sẽ tự động giết process cũ

---

## 🔧 Troubleshooting

**Q: Port 17891 already in use?**
```powershell
Get-NetTCPConnection -LocalPort 17891 | Select-Object OwningProcess
taskkill /PID <PID> /F
```

**Q: Muốn đổi port?**
Sửa trong [ExamGuard.Agent/Program.cs](ExamGuard.Agent/Program.cs):
```csharp
app.Urls.Add("http://127.0.0.1:17891");  // Đổi port ở đây
```

---

## 📦 File được publish

- **Executable**: `d:\Remote Desktop\ExamGuard.Agent\bin\Release\net8.0\win-x64\publish\ExamGuard.Agent.exe`
- **Size**: Single-file executable (~100MB), không cần dotnet runtime
- **OS**: Windows x64

Chạy ngay được, không cần cài đặt gì thêm!
