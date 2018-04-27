@echo off

rem Build and pack to nuget a solution Binary
rem Copyright (c) Davide Gironi, 2014

powershell -Command "& { [Console]::WindowWidth = 150; [Console]::WindowHeight = 50; Start-Transcript out.txt; Import-Module .\Tools\PSake\psake.psm1; Invoke-psake '.\Tools\AutoBuilder\AutoBuilder.ps1' -task Build; Remove-Module psake; %*; Stop-Transcript; exit $lastexitcode}"

.\Tools\NuGet\nuget.exe pack ..\AdvancedDataGridView\AdvancedDataGridView.csproj -Properties Configuration=Release -OutputDirectory ..\..\Release\nupkg

powershell -Command "& { [Console]::WindowWidth = 150; [Console]::WindowHeight = 50; Start-Transcript out.txt; Import-Module .\Tools\PSake\psake.psm1; Invoke-psake '.\Tools\AutoBuilder\AutoBuilder.ps1' -task Clean; Remove-Module psake; %*; Stop-Transcript; exit $lastexitcode}"

pause
