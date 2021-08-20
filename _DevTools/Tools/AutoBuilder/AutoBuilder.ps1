#-------------------------------------
# AutoBuild 1.0.2.2
# Copyright (c) 2012 Davide Gironi
#
# a Build automation script that runs on psake (https://github.com/psake/psake)
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
	$zipFileNameSrc = $zipFileNameSrcPrefix + "_" + $version + ".zip";
	$zipFileNameBin = $zipFileNameBinPrefix + "_" + $version + ".zip";
	$zipFileNameDbg = $zipFileNameDbgPrefix + "_" + $version + ".zip";
		
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
	ForEach ($build in $builds)
	{
		$name = $build.Name
		
		Write-Host -ForegroundColor Green "Clean Debug " $name
		Write-Host
		exec { msbuild "/t:clean" "/p:Configuration=Debug" ".\$name.sln" | Out-Default } "Error building $name"
		
		Write-Host -ForegroundColor Green "Clean Release " $name
		Write-Host
		exec { msbuild "/t:clean" "/p:Configuration=Release" ".\$name.sln" | Out-Default } "Error building $name"
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
	ForEach ($build in $builds)
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
#Task: update Versions
#-------------------------------------
task UpdateVersion {
	#record running location
	Push-Location -Path $Temp
		
	#set working location
	Set-Location $sourceDir

	#update version
	Write-Host -ForegroundColor Green "Updating version"
	Write-Host
	Update-VersionAssemblyInfoFiles $sourceDir $version
	#build solutions
	ForEach ($build in $builds)
	{
		$name = $build.Name
		Update-VersionProjectFiles "$sourceDir\$name.sln" $version
	}	
	
	#pop running location
	Pop-Location
	
	Write-Host
}

#-------------------------------------
#Task: Build solutions
#-------------------------------------
task Build -depends Clean {
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
	ForEach ($build in $builds)
	{
		$name = $build.Name
		
		$buildConstants = $build.Constants
		$defineConstants = ""
		if ($buildConstants -ne "")
		{
			$defineConstants = "/p:DefineConstants=`"$buildConstants`""
		}
				
		if ($buildDebugAndRelease -eq $true)
		{
			Write-Host -ForegroundColor Green "Building Debug " $name
			Write-Host
			exec { msbuild "/t:restore" "/p:Configuration=Debug" ".\$name.sln" | Out-Default } "Error restoring $name"
			exec { msbuild "/t:build" "/p:Configuration=Debug" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "$defineConstants" "/m" ".\$name.sln" | Out-Default } "Error building $name"
		}
		
		Write-Host -ForegroundColor Green "Building Release " $name
		Write-Host
		exec { msbuild "/t:restore" "/p:Configuration=Release" ".\$name.sln" | Out-Default } "Error restoring $name"
		exec { msbuild "/t:build" "/p:Configuration=Release" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "$defineConstants" "/m" ".\$name.sln" | Out-Default } "Error building $name"
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
	ForEach ($build in $builds)
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
			
			Write-Host -ForegroundColor Green "$projectFile"
			
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
			
			#check if is SDK project
			$isSdk = Select-String -Path "$sourceDir\$projectFile" -Pattern '<Project Sdk="Microsoft.NET.Sdk">' -SimpleMatch -Quiet
			
			#copy project release files
			robocopy @("$projectDirectory\bin\Release", "$workingDir\Bin\$projectName", '*.*', '/S', '/NP', '/XO', '/XF', '*.pdb', '*.xml') | Out-Default
			
			#copy project debug files
			if ($releaseDebug -eq $true)
			{
				robocopy @("$projectDirectory\bin\Debug", "$workingDir\Dbg\$projectName", '*.*', '/S', '/NP', '/XO') | Out-Default
			}
			
			#copy optionals files for the release
			ForEach ($releasebinincludefile in $releasebinincludefiles)
			{
				$filesitems = $releasebinincludefile.Files
				if ($projectName -eq $releasebinincludefile.Name)
				{
					ForEach ($fileitem in $filesitems)
					{
						$filenamefromitem = $fileitem.FileNameFrom
						if ($fileitem.FileNameTo -ne "")
						{
							$filenametoitem = $fileitem.FileNameTo
							
							#set the list of file to be copied
							$filenametoitemsBin = @()
							$filenametoitemsDbg = @()
							if ($isSdk)
							{
								Get-ChildItem -Path "$projectDirectory\bin\Release" -Directory |
									ForEach-Object {
										$sdkframework = $_.Name
										$filenametoitemsBin += "$workingDir\Bin\$projectName\$sdkframework\$filenametoitem"
										$filenametoitemsDbg += "$workingDir\Dbg\$projectName\$sdkframework\$filenametoitem"
									}
							}
							else
							{
								$filenametoitemsBin += "$workingDir\Bin\$projectName\$filenametoitem"
								$filenametoitemsDbg += "$workingDir\Dbg\$projectName\$filenametoitem"
							}
							
							ForEach ($filetoname in $filenametoitemsBin)
							{
								#copy optional release files
								Write-Host "Copying $projectDirectory\$filenamefromitem to $filetoname"
								try {
									New-Item -ItemType File -Path "$filetoname " -Force
								}
								catch { }
								Copy-Item "$projectDirectory\$filenamefromitem" "$filetoname" -Force -Recurse
							}
							
								
							#copy optional debug files
							if ($releaseDebug -eq $true)
							{
								ForEach ($filetoname in $filenametoitemsDbg)
								{
									#copy optional release files
									Write-Host "Copying $projectDirectory\$filenamefromitem to $filetoname"
									try {
										New-Item -ItemType File -Path "$filetoname " -Force
									}
									catch { }
									Copy-Item "$projectDirectory\$filenamefromitem" "$filetoname" -Force -Recurse
								}
							}
						}						
					}
				}
			}			
		}
	}

	#ReleaseBinCmd commands
	ForEach ($build in $builds)
	{
		$cmds = $build.ReleaseBinCmd
		ForEach ($cmd in $cmds)
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
	ForEach ($build in $builds)
	{
		$cmds = $build.ReleaseSrcCmd
		ForEach ($cmd in $cmds)
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
#Task: Run TestConsole
#-------------------------------------
task TestConsole -depends Test
#-------------------------------------
#Task: Run Test
#-------------------------------------
task Test -depends Build {
	ForEach ($build in $builds)
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
			
			ForEach ($test in $tests)
			{
				if ($projectName -eq $test.Name)
				{
					Write-Host -ForegroundColor Green "Running tests " $test.Name
										
					exec { dotnet test "$sourceDir\$projectFile" -c "Release" | Out-Default }
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
function Update-VersionAssemblyInfoFiles ([string] $sourceDir, [string] $versionNumber) {
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$assemblyVersion = 'AssemblyVersion("' + $versionNumber + '")'
	$assemblyFileVersion = 'AssemblyFileVersion("' + $versionNumber + '")'
	
	Get-ChildItem -Path $sourceDir -r -filter AssemblyInfo.cs | ForEach-Object {
		$filename = $_.Directory.ToString() + '\' + $_.Name
		
		Write-Host $filename ' -> ' $versionNumber
		
		$filenameContent = Get-Content $filename
		if ($filenameContent)
		{
			try
			{
				$filenameContent | ForEach-Object {
				% {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
				% {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion }
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

#update project files with the given version numbers
function Update-VersionProjectFiles ([string] $solutionFile, [string] $versionNumber) {	
	$projects = GetProjects $solutionFile
	ForEach ($project in $projects)
	{
		$projectFile = $project.File
		
		Write-Host $projectFile ' -> ' $versionNumber
		
		# Read the existing file
		[xml]$xmlDoc = Get-Content $projectFile

		# If it was one specific element you can just do like so:
		if ($xmlDoc.Project)
		{
			if ($xmlDoc.Project.PropertyGroup)
			{
				if ($xmlDoc.Project.PropertyGroup.Version -ne $null)
				{
					$xmlDoc.Project.PropertyGroup.Version = $versionNumber
				}
				if ($xmlDoc.Project.PropertyGroup.AssemblyVersion -ne $null)
				{
					$xmlDoc.Project.PropertyGroup.AssemblyVersion = $versionNumber
				}
				if ($xmlDoc.Project.PropertyGroup.FileVersion -ne $null)
				{
					$xmlDoc.Project.PropertyGroup.FileVersion = $versionNumber
				}
				if ($xmlDoc.Project.PropertyGroup.PackageVersion -ne $null)
				{
					$xmlDoc.Project.PropertyGroup.PackageVersion = $versionNumber
				}
			}	
		}
		
		$xmlDoc.Save("$sourceDir\$projectFile")
	}
}
