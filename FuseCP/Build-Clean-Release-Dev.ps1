# forced output into English
$env:DOTNET_CLI_UI_LANGUAGE = "en-US"
$env:VSLANG = "1033"

$scriptPath = ((Get-Item -Path ".\").FullName)
$FuseCP_Path = "C:\FuseCP"
Write-Host "Current script directory is `'$scriptPath`'" -ForegroundColor Yellow

#########################################################################################################################################################
function CleanBuildFolder()  # Function to clean the files in the specified build folder
{
	Param(
		[string]$Folder       # The folder to delete all files from only if it exists
	)
	if (Test-Path $Folder) {
		foreach ($dItem in (Get-ChildItem -Path $Folder)) {
			Remove-Item $dItem.FullName -Recurse -Force
		}
		Write-Host " Cleaning the `"$Folder`" directory" -ForegroundColor Green
	}
}

# Run the above function to clean the build directories specified below
CleanBuildFolder -Folder "$scriptPath\Bin"
CleanBuildFolder -Folder "$scriptPath\Build\Release\AWStats.Viewer"
CleanBuildFolder -Folder "$scriptPath\Build\Release\EnterpriseServer"
CleanBuildFolder -Folder "$scriptPath\Build\Release\FixDefaultPublicFolderMailbox"
CleanBuildFolder -Folder "$scriptPath\Build\Release\HyperVUtils"
CleanBuildFolder -Folder "$scriptPath\Build\Release\Import.CsvBulk"
CleanBuildFolder -Folder "$scriptPath\Build\Release\Import.Enterprise"
CleanBuildFolder -Folder "$scriptPath\Build\Release\Installer"
CleanBuildFolder -Folder "$scriptPath\Build\Release\LocalizationToolkit"
CleanBuildFolder -Folder "$scriptPath\Build\Release\Portal"
CleanBuildFolder -Folder "$scriptPath\Build\Release\SchedulerService"
CleanBuildFolder -Folder "$scriptPath\Build\Release\Server"
CleanBuildFolder -Folder "$scriptPath\Build\Release\VMConfig"
CleanBuildFolder -Folder "$scriptPath\Build\Release\WebDavPortal"
CleanBuildFolder -Folder "$scriptPath\Build\Release\WiXInstaller"
CleanBuildFolder -Folder "$scriptPath\Deploy\Release\Database"
CleanBuildFolder -Folder "$scriptPath\Deploy\Release\Install"
CleanBuildFolder -Folder "$scriptPath\Deploy\Release\Tools"
CleanBuildFolder -Folder "$scriptPath\Deploy\Release\Update"
if (Test-Path "$scriptPath\Deploy\Release") {
	Get-ChildItem "$scriptPath\Deploy\Release" -File -Filter "*.zip" | Remove-Item -Force
}
Remove-Item -Path "$scriptPath\Deploy\Release\FuseCP.msi" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$scriptPath\Deploy\Release\FuseCPSetup.exe" -Force -ErrorAction SilentlyContinue

# Remove the msbuild.log file before building

if (Test-Path "$scriptPath\msbuild.log") {
	Remove-Item "$scriptPath\msbuild.log" -Recurse -Force
	Write-Host " Deleting the `"$scriptPath\msbuild.log`" file" -ForegroundColor Green
}
Start-Sleep -Seconds 5
#########################################################################################################################################################
# Start the build process
Write-Host "`n Starting Build Process`n`n" -ForegroundColor Green
#Invoke-Expression -Command: "& $(((Get-Content "$scriptPath\build-release.bat").Replace('%windir%', $env:windir).Replace('%ProgramFiles(x86)%', ${env:ProgramFiles(x86)}))[4])"
$batFilePath = "$scriptPath\build-release.bat"
Invoke-Expression -Command: "& $batFilePath"

# Check the "msbuild.log" file for any errors and display then on the screen
if ([bool]((Get-Content "$scriptPath\msbuild.log") -match ' error ')) {
	Write-Host "`n`n`n The following errors were found`n" -ForegroundColor Yellow
	Write-Host "$((Get-Content "$scriptPath\msbuild.log") -match ': error ') `n`n" -ForegroundColor Red
}else{
	Write-Host "Current directory: `'$((Get-Location).Path)`'"
	Write-Host "`n`n`n Copying the config files over" -ForegroundColor Cyan
	# Copy the "Enterprise Server" web.config over as long as it has been installed by the FuseCP Installer
	if (Test-Path "$FuseCP_Path\Enterprise Server\web.config") {
		Copy-Item -Path "$FuseCP_Path\Enterprise Server\web.config" -Destination "$scriptPath\Build\Release\EnterpriseServer" -Recurse -force
		write-host " Enterprise Server - web.config" -ForegroundColor Green
	}
	# Copy the "Portal" web.config over as long as it has been installed by the FuseCP Installer
	if (Test-Path "$FuseCP_Path\Portal\web.config") {
		Copy-Item -Path "$FuseCP_Path\Portal\web.config" -Destination "$scriptPath\Build\Release\Portal" -Recurse -force
		write-host " Portal - web.config" -ForegroundColor Green
	}
	# Copy the "Portal" SiteSettings.config over as long as it has been installed by the FuseCP Installer
	if (Test-Path "$FuseCP_Path\Portal\App_Data\SiteSettings.config") {
		Copy-Item -Path "$FuseCP_Path\Portal\App_Data\SiteSettings.config" -Destination "$scriptPath\Build\Release\Portal\App_Data" -Recurse -force
		write-host " Portal - SiteSettings.config" -ForegroundColor Green
	}
	# Copy the "Server" web.config over as long as it has been installed by the FuseCP Installer
	if (Test-Path "$FuseCP_Path\Server\web.config") {
		Copy-Item -Path "$FuseCP_Path\Server\web.config" -Destination "$scriptPath\Build\Release\Server" -Recurse -force
		write-host " Server - web.config" -ForegroundColor Green
	}
	# Copy the "Cloud Storage Portal" web.config over as long as it has been installed by the FuseCP Installer
	if (Test-Path "$FuseCP_Path\Cloud Storage Portal\web.config") {
		Copy-Item -Path "$FuseCP_Path\Cloud Storage Portal\web.config" -Destination "$scriptPath\Build\Release\WebDavPortal" -Recurse -force
		write-host " Cloud Storage Portal (WebDav) - web.config" -ForegroundColor Green
	}
}

# Kill off the processes that don't automatically close from the build process
Write-Host "`n Closing the Build Processes that are left behind" -ForegroundColor Cyan
$BuildProcesses = @("MSBuild", "MSBuildTaskHost", "dotnet", "VBCSCompiler")
foreach ($procName in $BuildProcesses) {
    $foundProcs = Get-Process -Name $procName -ErrorAction SilentlyContinue
    if ($foundProcs) {
        Write-Host " Closing: $procName" -ForegroundColor Gray
        $foundProcs | Stop-Process -Force -ErrorAction SilentlyContinue
    }
}

# Prompt the user to press any key to exit once the script has processed
if ($psISE) { # Check if running Powershell ISE
	Add-Type -AssemblyName System.Windows.Forms
	[System.Windows.Forms.MessageBox]::Show("Press any key to exit")
	exit
}else{
	Write-Host "`n  Press any key to exit..." -ForegroundColor Yellow
	$x = $host.ui.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	exit
}

