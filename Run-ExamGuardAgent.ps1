# ExamGuard Agent - Script chạy nhanh
# Chạy: .\Run-ExamGuardAgent.ps1

$agentPath = "d:\Remote Desktop\ExamGuard.Agent\bin\Release\net8.0\win-x64\publish\ExamGuard.Agent.exe"

Write-Host "================================"
Write-Host "ExamGuard Agent - Quét Remote Desktop"
Write-Host "================================"
Write-Host ""
Write-Host "Agent sẽ lắng nghe trên: http://127.0.0.1:17891"
Write-Host ""
Write-Host "Endpoint:"
Write-Host "  GET  http://127.0.0.1:17891/health"
Write-Host "  POST http://127.0.0.1:17891/check"
Write-Host ""
Write-Host "Nhấn Ctrl+C để dừng"
Write-Host "================================"
Write-Host ""

# Nếu port đã được sử dụng, giết process cũ
$port = 17891
$existingProcess = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
if ($existingProcess) {
    Write-Host "Port $port đang được sử dụng, dừng process cũ..."
    Get-Process -Id $existingProcess.OwningProcess -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 1
}

# Chạy agent
& $agentPath
