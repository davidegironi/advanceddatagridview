@echo off

rem Build and Release a solution Binary and Source
rem Copyright (c) Davide Gironi, 2014

SET NOPAUSE=1
cmd /C run-cleanfolders.bat
if ERRORLEVEL 1 (
	pause
	exit %ERRORLEVEL%
)
cmd /C run-releasebin.bat
if ERRORLEVEL 1 (
	pause
	exit %ERRORLEVEL%
)
cmd /C run-releasesrc.bat
if ERRORLEVEL 1 (
	pause
	exit %ERRORLEVEL%
)
pause
