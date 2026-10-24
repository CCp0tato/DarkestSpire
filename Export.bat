@echo off
rem Double-click to build + export the mod. Pass args through, e.g. Export.bat -OutDir dist
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Export.ps1" %*
if errorlevel 1 (
    pause
    exit /b 1
)
if "%~1"=="" pause
