<div align="center">
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/vue/vue.png" width="80" alt="Logo" />
  <h1>ExamGuard System 🛡️</h1>
  <p>A lightweight, powerful prototype for simulating an online exam environment guard workflow.</p>

  <!-- Badges -->
  <p>
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 8" />
    <img src="https://img.shields.io/badge/Vue.js-3.5-4FC08D?style=for-the-badge&logo=vue.js" alt="Vue 3" />
    <img src="https://img.shields.io/badge/Vite-8.1-646CFF?style=for-the-badge&logo=vite" alt="Vite" />
    <img src="https://img.shields.io/badge/TailwindCSS-4.3-38B2AC?style=for-the-badge&logo=tailwind-css" alt="Tailwind" />
  </p>
</div>

---

## 📥 Download ExamGuard Agent

You can download the pre-compiled, standalone **ExamGuard Agent** tools directly by clicking the links below. No installation required!

| Platform | Architecture | Download Link |
| :--- | :--- | :--- |
| **🪟 Windows** | x64 (64-bit) | [⬇️ Tải xuống Agent (win-x64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/win-x64/ExamGuard.Agent.exe) |
| | x86 (32-bit) | [⬇️ Tải xuống Agent (win-x86)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/win-x86/ExamGuard.Agent.exe) |
| | ARM64 (Snapdragon) | [⬇️ Tải xuống Agent (win-arm64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/win-arm64/ExamGuard.Agent.exe) |
| **🍎 macOS** | Apple Silicon (M1/M2/M3) | [⬇️ Tải xuống Agent (osx-arm64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/osx-arm64/ExamGuard.Agent) |
| | Intel (x64) | [⬇️ Tải xuống Agent (osx-x64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/osx-x64/ExamGuard.Agent) |
| **🐧 Linux** | x64 | [⬇️ Tải xuống Agent (linux-x64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/linux-x64/ExamGuard.Agent) |
| | ARM64 (Raspberry Pi, v.v.) | [⬇️ Tải xuống Agent (linux-arm64)](https://github.com/Phan-Thanh-Danh/Remote-Desktop/raw/main/publish/linux-arm64/ExamGuard.Agent) |

> ⚠️ **Note for macOS / Linux users:** After downloading, you may need to make the file executable by running `chmod +x ExamGuard.Agent` in your terminal.

---

## 🏗️ Technology Stack

- **Backend:** ASP.NET Core Web API (C#)
- **Frontend:** Vue 3 + Vite + Tailwind CSS
- **Agent Client:** Standalone .NET Core App (Cross-platform)

## 🚀 Running Locally (For Developers)

### 1. Run Backend API
```bash
cd ExamGuard.Api
dotnet run --urls http://localhost:5204
```

### 2. Run Frontend Web
```bash
cd examguard-fe
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

### 3. Run ExamGuard Agent locally (Source)
```powershell
.\Run-ExamGuardAgent.ps1
```

---

## 📝 Demo Flow

1. Open **[http://localhost:5173/environment-check](http://localhost:5173/environment-check)**
2. Create a new guard session.
3. Click `"Kiểm tra bằng ExamGuard Agent"` — this will call the Agent running locally at `POST http://127.0.0.1:17891/check`.
4. The frontend reads the updated status from the backend before allowing entry into the mock exam environment.

---

## 🔌 API Summary

### Core Services
- `POST /api/exam-guard/sessions` - Initialize a new session
- `GET /api/exam-guard/sessions/{sessionId}/status` - Retrieve session status
- `POST /api/exam-guard/sessions/{sessionId}/report` - Submit a violation report
- `POST /api/exam-guard/sessions/{sessionId}/heartbeat` - Maintain connection heartbeat

### Agent Service
- `POST http://127.0.0.1:17891/check` - Local environment check trigger
