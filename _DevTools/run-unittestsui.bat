@echo off

rem Run unit tests by GUI for a solution
rem Copyright (c) Davide Gironi, 2014

powershell -Command "& { [Console]::WindowWidth = 150; [Console]::WindowHeight = 50; Start-Transcript out.txt; Import-Module .\Tools\PSake\psake.psm1; Invoke-psake '.\Tools\AutoBuilder\AutoBuilder.ps1' -task TestUi; Remove-Module psake; %*; Stop-Transcript; exit $lastexitcode}"
@if "%NOPAUSE%"=="1" (echo stop) else (pause)
exit %errorlevel%
