@echo off
setlocal
cd /d "%~dp0"
echo Starting local Add-Deduct APP...
echo If this window reports an error, please take a screenshot.
powershell.exe -NoProfile -File "%~dp0start-local.ps1"
echo.
echo Local server has stopped or failed to start.
pause
