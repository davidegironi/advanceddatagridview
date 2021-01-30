#-------------------------------------
# AutoBuild 1.0.1.9
# Copyright (c) 2012 Davide Gironi
#
# a Build automation script which runs on psake (https://github.com/psake/psake)
# 
# Please refer to LICENSE file for licensing information.
#-------------------------------------

Framework "4.5.1"

#-------------------------------------
#Main Properties
#-------------------------------------
properties {
	#set location to caller path
	Set-Location .\..\..\
	
	#load projects variables
	. .\AutoBuilder.config.ps1
	
	#directories setup
	$sourceDir = $baseDir
	$workingDir = Resolve-Path .\Working
	if (-Not (Test-Path variable:local:releaseDir))
	{
		$releaseDir = Resolve-Path .\Release
	}
	
	#filename for package Source and Binary files
	$zipFileNameSrcPrefix = "$solutionName-src";
	$zipFileNameBinPrefix = "$solutionName-bin";
	$zipFileNameDbgPrefix = "$solutionName-dbg";
	$zipFileNameSrc = $zipFileNameSrcPrefix + "_" + $fileVersion + ".zip";
	$zipFileNameBin = $zipFileNameBinPrefix + "_" + $fileVersion + ".zip";
	$zipFileNameDbg = $zipFileNameDbgPrefix + "_" + $fileVersion + ".zip";
		
	#global variable for Test
	$testconsole = 1
}


#-------------------------------------
#Task: Default
#-------------------------------------
task default -depends Build


#-------------------------------------
#Task: CleanWorking clean working folder
#-------------------------------------
task CleanWorking {
	if (Test-Path -path $workingDir)
	{
		Write-Output "Deleting Working Directory"
		Remove-Item -Recurse -Force $workingDir
	}
	New-Item -Path $workingDir -ItemType Directory
	
	Write-Host
}


#-------------------------------------
#Task: Clean solutions
#-------------------------------------
task Clean {
	#record running location
	Push-Location -Path $Temp
		
	#set working location
	Set-Location $sourceDir

	#build solutions
	foreach ($build in $builds)
	{
		$name = $build.Name
		
		Write-Host -ForegroundColor Green "Clean Debug " $name
		Write-Host
		exec { msbuild "/t:Clean" "/p:Configuration=Debug" "/p:OutputPath=bin\Debug\" ".\$name.sln" | Out-Default } "Error building $name"
		
		Write-Host -ForegroundColor Green "Clean Release " $name
		Write-Host
		exec { msbuild "/t:Clean" "/p:Configuration=Release" "/p:OutputPath=bin\Release\" ".\$name.sln" | Out-Default } "Error building $name"
	}
	
	#pop running location
	Pop-Location
	
	Write-Host
}


#-------------------------------------
#Task: Clean solutions build folders
#-------------------------------------
task CleanFolders -depends Clean {
	#record running location
	Push-Location -Path $Temp
		
	#set working location
	Set-Location $sourceDir

	#build solutions
	foreach ($build in $builds)
	{
		$name = $build.Name
		
		Write-Host -ForegroundColor Green "Clean Folders " $name
		Write-Host
			
		$projects = GetProjects "$sourceDir\$name.sln"
		ForEach ($project in $projects)
		{
			$projectFile = $project.File
			$projectDirectoryFileInfo = Get-ChildItem "$sourceDir\$projectFile"
			$projectDirectory = $projectDirectoryFileInfo.DirectoryName
					
			try
			{
				Write-Host "Deleting folder $projectDirectory\bin"
				Remove-Item -Recurse -Force "$projectDirectory\bin"
			} catch { }
			try
			{
				Write-Host "Deleting folder $projectDirectory\obj"
				Remove-Item -Recurse -Force "$projectDirectory\obj"
			} catch { }
		}
	}
	
	#pop running location
	Pop-Location
	
	Write-Host
}


#-------------------------------------
#Task: update Assembly Version
#-------------------------------------
task UpdateVersion {
	#record running location
	Push-Location -Path $Temp
		
	#set working location
	Set-Location $sourceDir

	#update assembly version
	Write-Host -ForegroundColor Green "Updating assembly version"
	Write-Host
	Update-AssemblyInfoFiles $sourceDir $assemblyVersion $fileVersion
		
	#pop running location
	Pop-Location
	
	Write-Host
}

#-------------------------------------
#Task: Build solutions
#-------------------------------------
task Build {
	#check internal task request variable
	if ($builddebugandreleaseint -eq $true)
	{
		$buildDebugAndRelease = $true
	}
	
	#record running location
	Push-Location -Path $Temp
		
	#set working location
	Set-Location $sourceDir
	
	#build solutions
	foreach ($build in $builds)
	{
		$name = $build.Name
		
		$buildConstants = $build.Constants
		$defineConstants = "/p:DefineConstants=`"CODE_ANALYSIS;TRACE;$buildConstants`""
		
		if ($buildDebugAndRelease -eq $true)
		{
			Write-Host -ForegroundColor Green "Building Debug " $name
			Write-Host
			exec { msbuild "/t:Clean;Rebuild" "/p:Configuration=Debug" "/p:Platform=Any CPU" "/p:OutputPath=bin\Debug\" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "$defineConstants" ".\$name.sln" | Out-Default } "Error building $name"
		}
		
		Write-Host -ForegroundColor Green "Building Release " $name
		Write-Host
		exec { msbuild "/t:Clean;Rebuild" "/p:Configuration=Release" "/p:Platform=Any CPU" "/p:OutputPath=bin\Release\" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "$defineConstants" ".\$name.sln" | Out-Default } "Error building $name"
	}
	
	#pop running location
	Pop-Location
	
	Write-Host
}

#-------------------------------------
#Task: Build solutions
#-------------------------------------
task BuildDebugAndRelease -depends BuildDebugAndReleaseSet, Build
task BuildDebugAndReleaseSet {
	$script:builddebugandreleaseint = $true
	'$builddebugandreleaseint = ' + $script:builddebugandreleaseint
}

#-------------------------------------
#Task: Release the Binary files
#-------------------------------------
task ReleaseBin -depends CleanWorking, UpdateVersion, Build {
	Write-Host -ForegroundColor Green  "Grab Binary files"
	Write-Host
	
	if ($buildDebugAndRelease -eq $true -and $releaseDebugFiles -eq $true)
	{
		$releaseDebug = $true
	}
	
	#copy bin files
	foreach ($build in $builds)
	{
		$name = $build.Name
		$releasebinincludefiles = $build.ReleaseBinIncludeFiles
		$releasebinexcludeprojects = $build.ReleaseBinExcludeProjects
		
		$projects = GetProjects "$sourceDir\$name.sln"
		ForEach ($project in $projects)
		{
			$projectFile = $project.File
			$projectName = $project.Name
			$projectDirectoryFileInfo = Get-ChildItem "$sourceDir\$projectFile"
			$projectDirectory = $projectDirectoryFileInfo.DirectoryName
			
			#check project inclusion
			$excludefromrelease = $false
			ForEach ($releasebinexcludeproject in $releasebinexcludeprojects)
			{
				if ($releasebinexcludeproject.Name -eq $projectName)
				{
					$excludefromrelease = $true
					break
				}
			}
			if ($excludefromrelease -eq $true)
			{
				continue
			}
			
			#copy project release files
			robocopy @("$projectDirectory\bin\Release", "$workingDir\Bin\$projectName", '*.*', '/S', '/NP', '/XO', '/XF', '*.pdb', '*.xml') | Out-Default
			
			#copy project debug files
			if ($releaseDebug -eq $true)
			{
				robocopy @("$projectDirectory\bin\Debug", "$workingDir\Dbg\$projectName", '*.*', '/S', '/NP', '/XO') | Out-Default
			}
			
			#copy optionals files for the release
			foreach ($releasebinincludefile in $releasebinincludefiles)
			{
				$filesitems = $releasebinincludefile.Files
				if ($projectName -eq $releasebinincludefile.Name)
				{
					foreach ($fileitem in $filesitems)
					{
						$filenamefromitem = $fileitem.FileNameFrom
						if ($fileitem.FileNameTo -ne "")
						{
							#copy optional release files
							$filenametoitem = $fileitem.FileNameTo
							Write-Host "Copying $projectDirectory\$filenamefromitem to $workingDir\Bin\$projectName\$filenametoitem"
							try {
								New-Item -ItemType File -Path "$workingDir\Bin\$projectName\$filenametoitem " -Force
							}
							catch { }
							Copy-Item "$projectDirectory\$filenamefromitem" "$workingDir\Bin\$projectName\$filenametoitem" -Force -Recurse
							
							#copy optional debug files
							if ($releaseDebug -eq $true)
							{
								Write-Host "Copying $projectDirectory\$filenamefromitem to $workingDir\Dbg\$projectName\$filenametoitem"
								try {
									New-Item -ItemType File -Path "$workingDir\Dbg\$projectName\$filenametoitem " -Force
								}
								catch { }
								Copy-Item "$projectDirectory\$filenamefromitem" "$workingDir\Dbg\$projectName\$filenametoitem" -Force -Recurse
							}
						}						
					}
				}
			}			
		}
	}

	#ReleaseBinCmd commands
	foreach ($build in $builds)
	{
		$cmds = $build.ReleaseBinCmd
		foreach ($cmd in $cmds)
		{
			$cmdexec = $cmd.Cmd
			Write-Host -ForegroundColor Green "Executing command " $cmdexec
			Write-Host
			cmd /c $cmdexec
		}
	}
	
	#package bin files
	Write-Host -ForegroundColor Green  "Package Binary files"
	Write-Host
	exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileNameBin $workingDir\Bin\* | Out-Default } "Error zipping"
	if ($releaseDebug -eq $true)
	{
		Write-Host -ForegroundColor Green  "Package Debug files"
		Write-Host
		exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileNameDbg $workingDir\Dbg\* | Out-Default } "Error zipping"
	}
	
	#set release bin folder
	#maintain last release and forget $versionBuild
	$releaseVersion = GetVersion $versionMajor $versionMinor "x" $versionRevision
	$releaseDest = "$releaseDir\$releaseVersion"
	
	#remove older release for the actual version, or create new release folder
	if (Test-Path $releaseDest)
	{
		if ($removeElderReleaseWithSameVersion -eq $true)
		{
			Remove-Item $releaseDest\$zipFileNameBinPrefix* -recurse
			if ($releaseDebug -eq $true)
			{
				Remove-Item $releaseDest\$zipFileNameDbgPrefix* -recurse
			}			
		}
	}
	else
	{
		New-Item -ItemType directory -Path $releaseDest
	}
	
	#move package file
	Write-Host -ForegroundColor Green  "Move package release file to Release Dir"
	Write-Host
	Move-Item $workingDir\$zipFileNameBin $releaseDest\$zipFileNameBin -Force
	if ($releaseDebug -eq $true)
	{
		Write-Host -ForegroundColor Green  "Move package debug file to Release Dir"
		Write-Host
		Move-Item $workingDir\$zipFileNameDbg $releaseDest\$zipFileNameDbg -Force
	}
	
	Write-Host
}


#-------------------------------------
#Task: Release the Source files
#-------------------------------------
task ReleaseSrc -depends CleanWorking, UpdateVersion {
	Write-Host -ForegroundColor Green  "Grab Source files"
	Write-Host
	
	#copy src files
	robocopy @("$sourceDir", "$workingDir\Src", '/MIR', '/NP', '/XD', '".vs"', '".svn"', '"bin"', '"obj"', '"TestResults"', '"AppPackages"') + $releaseSrcExcludeFolders + @('/XF', '"*.suo"', '"*.user"') + $releaseSrcExcludeFiles | Out-Default

	#ReleaseSrcCmd commands
	foreach ($build in $builds)
	{
		$cmds = $build.ReleaseSrcCmd
		foreach ($cmd in $cmds)
		{
			$cmdexec = $cmd.Cmd
			Write-Host -ForegroundColor Green "Executing command " $cmdexec
			Write-Host
			cmd /c $cmdexec
		}
	}
	
	#package src files
	Write-Host -ForegroundColor Green  "Package Source files"
	Write-Host
	exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileNameSrc $workingDir\Src\* | Out-Default } "Error zipping"
	
	#set release bin folder
	#maintain last release and forget $versionBuild
	$releaseVersion = GetVersion $versionMajor $versionMinor "x" $versionRevision
	$releaseDest = "$releaseDir\$releaseVersion"
	
	#remove older release for the actual version, or create new release folder
	if (Test-Path $releaseDest)
	{
		if ($removeElderReleaseWithSameVersion -eq $true)
		{
			Remove-Item $releaseDest\$zipFileNameSrcPrefix* -recurse
		}
	}
	else
	{
		New-Item -ItemType directory -Path $releaseDest
	}
	
	#move package file
	Write-Host -ForegroundColor Green  "Move package file to Release Dir"
	Write-Host
	Move-Item $workingDir\$zipFileNameSrc $releaseDest\$zipFileNameSrc -Force
	
	Write-Host
}


#-------------------------------------
#Task: Run TestUI
#-------------------------------------
task TestUi -depends TestUiSet,Test
task TestUiSet {
	$script:testconsole = 0
	'$testconsole = ' + $script:testconsole
}
#-------------------------------------
#Task: Run TestConsole
#-------------------------------------
task TestConsole -depends TestConsoleSet,Test
task TestConsoleSet {
	$script:testconsole = 1
	'$testconsole = ' + $script:testconsole
}
#-------------------------------------
#Task: Run Test
#-------------------------------------
task Test -depends Build {
	foreach ($build in $builds)
	{
		$name = $build.Name
		$tests = $build.Tests
				
		#loop projects
		$projects = GetProjects "$sourceDir\$name.sln"
		ForEach ($project in $projects)
		{
			$projectFile = $project.File
			$projectName = $project.Name
			$projectDirectoryFileInfo = Get-ChildItem "$sourceDir\$projectFile"
			$projectDirectory = $projectDirectoryFileInfo.DirectoryName
			
			foreach ($test in $tests)
			{
				$testdll = $test.TestDll
				if ($projectName -eq $test.Name)
				{
					Write-Host -ForegroundColor Green "Running tests " $test.Name
					if ($script:testconsole -eq 1)
					{
						exec { .\Tools\NUnit\nunit-console.exe "$projectDirectory\bin\Release\$testdll" | Out-Default } "Error running $testname tests"
					}
					else
					{
						exec { .\Tools\NUnit\nunit.exe "$projectDirectory\bin\Release\$testdll" /run | Out-Default } "Error running $testname tests"
					}
				}
			}
		}
	}
	
	Write-Host
}


#-------------------------------------
#Utilites Functions
#-------------------------------------

#get project by solution sln file path
function GetProjects($solutionPath) {
	$results = @()
	Get-Content $solutionPath |
		Select-String 'Project\(' |
		ForEach-Object {
			$projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') };
			if ($projectParts[2] -match '\..*proj')
			{
				$results += New-Object PSObject -Property @{
					Name = $projectParts[1];
					File = $projectParts[2];
					Guid = $projectParts[3]
				}
			}
		}

	return $results
}

#get version build depending on current build date
function GetVersionBuild() {
	$now = [DateTime]::Now

	$year = $now.Year - 2000
	$month = $now.Month
	$totalMonthsSince2000 = ($year * 12) + $month
	$day = $now.Day
	$buildnum = "{0}{1:00}" -f $totalMonthsSince2000, $day

	return $buildnum
}

#get version
function GetVersion([string] $versionMajor, [string] $versionMinor, [string] $versionBuild, [string] $versionRevision)  {
	return $versionMajor + "." + $versionMinor + "." + $versionBuild + "." + $versionRevision
}

#update project assembly files with the given version numbers
function Update-AssemblyInfoFiles ([string] $sourceDir, [string] $assemblyVersionNumber, [string] $fileVersionNumber) {
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")'
	$fileVersion = 'AssemblyFileVersion("' + $fileVersionNumber + '")'
	
	Get-ChildItem -Path $sourceDir -r -filter AssemblyInfo.cs | ForEach-Object {
		$filename = $_.Directory.ToString() + '\' + $_.Name
		
		Write-Host $filename
		$filename + ' -> ' + $assemblyVersionNumber
		
		$filenameContent = Get-Content $filename
		if ($filenameContent)
		{
			try
			{
				$filenameContent | ForEach-Object {
				% {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
				% {$_ -replace $fileVersionPattern, $fileVersion }
				} | Out-File $filename -Force
			}
			catch { }
		}
		else
		{
			throw "AssemblyInfo.cs empty for file $filename"
		}
	}
}