@echo off
REM ExamGuard Agent - Script chạy nhanh (Windows)
REM Chạy: Run-ExamGuardAgent.bat

setlocal enabledelayedexpansion

set AGENT_PATH=d:\Remote Desktop\ExamGuard.Agent\bin\Release\net8.0\win-x64\publish\ExamGuard.Agent.exe

echo ================================
echo ExamGuard Agent - Quét Remote Desktop
echo ================================
echo.
echo Agent se lang nghe tren: http://127.0.0.1:17891
echo.
echo Endpoint:
echo   GET  http://127.0.0.1:17891/health
echo   POST http://127.0.0.1:17891/check
echo.
echo Nhan Ctrl+C de dung
echo ================================
echo.

"%AGENT_PATH%"
pause
