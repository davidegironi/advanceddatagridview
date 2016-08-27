@echo off

rem Initialize dev environment
rem Copyright (c) Davide Gironi, 2015

rem load custom dev environment commands
if exist config.run-devinit.bat config.run-devinit.bat

if not exist Working mkdir Working
if not exist ..\..\Release mkdir ..\..\Release

exit
