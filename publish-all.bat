@echo off
echo Đang build ExamGuard.Agent cho nhieu nen tang (Windows, Linux, macOS)...
echo.

set PROJECT_PATH=ExamGuard.Agent\ExamGuard.Agent.csproj
set OUTPUT_DIR=publish

set BUILD_FLAGS=-c Release --self-contained -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:IsTransformWebConfigDisabled=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true

echo [1] Build cho Windows x64...
dotnet publish "%PROJECT_PATH%" -r win-x64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\win-x64"

echo.
echo [2] Build cho Windows x86 (32-bit)...
dotnet publish "%PROJECT_PATH%" -r win-x86 %BUILD_FLAGS% -o "%OUTPUT_DIR%\win-x86"

echo.
echo [3] Build cho Windows ARM64 (Snapdragon/ARM)...
dotnet publish "%PROJECT_PATH%" -r win-arm64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\win-arm64"

echo.
echo [4] Build cho Linux x64...
dotnet publish "%PROJECT_PATH%" -r linux-x64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\linux-x64"

echo.
echo [5] Build cho Linux ARM64 (Raspberry Pi, AWS Graviton...)...
dotnet publish "%PROJECT_PATH%" -r linux-arm64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\linux-arm64"

echo.
echo [6] Build cho macOS x64 (Chip Intel)...
dotnet publish "%PROJECT_PATH%" -r osx-x64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\osx-x64"

echo.
echo [7] Build cho macOS ARM64 (Apple Silicon M1/M2/M3)...
dotnet publish "%PROJECT_PATH%" -r osx-arm64 %BUILD_FLAGS% -o "%OUTPUT_DIR%\osx-arm64"

echo.
echo Dang don dep cac file phu (pdb, web.config, staticwebassets...)...
del /s /q "%OUTPUT_DIR%\*.pdb" >nul 2>&1
del /s /q "%OUTPUT_DIR%\*.staticwebassets.endpoints.json" >nul 2>&1
del /s /q "%OUTPUT_DIR%\aspnetcorev2_inprocess.dll" >nul 2>&1
del /s /q "%OUTPUT_DIR%\web.config" >nul 2>&1

echo.
echo Hoan tat! Trong moi thu muc con bay gio chi co DUNG 1 FILE DUY NHAT (.exe cho Win, khong duoi cho Linux/macOS).
pause
