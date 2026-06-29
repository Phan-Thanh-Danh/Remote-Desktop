# ExamGuardDemo

A lightweight prototype for simulating an online exam environment guard workflow.

## Stack
- Backend: ASP.NET Core Web API
- Frontend: Vue 3 + Vite + Tailwind CSS

## Run backend
```bash
cd ExamGuard.Api
 dotnet run --urls http://localhost:5204
```

## Run frontend
```bash
cd examguard-fe
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

## Run ExamGuard Agent
```powershell
.\Run-ExamGuardAgent.ps1
```

## Demo flow
1. Open http://localhost:5173/environment-check
2. Create a guard session.
3. Click "Kiểm tra bằng ExamGuard Agent" to call `POST http://127.0.0.1:17891/check`.
4. The frontend reads the updated status from the backend before allowing entry into the fake exam.

## API summary
- POST /api/exam-guard/sessions
- GET /api/exam-guard/sessions/{sessionId}/status
- POST /api/exam-guard/sessions/{sessionId}/report
- POST /api/exam-guard/sessions/{sessionId}/heartbeat
- Agent: POST http://127.0.0.1:17891/check
