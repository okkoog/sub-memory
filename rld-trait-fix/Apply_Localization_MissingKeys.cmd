@echo off
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Apply_Localization_MissingKeys.ps1"
if errorlevel 1 echo ERROR: local key update failed.
pause
