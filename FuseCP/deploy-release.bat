@echo off
setlocal
pushd "%~dp0"

:: forced output into English
set DOTNET_CLI_UI_LANGUAGE=en-US
set VSLANG=1033

IF not defined SkipIISReset (
	where iisreset >nul 2>&1
	IF NOT ERRORLEVEL 1 (
		echo Running IIS reset to clear potential file locks...
		iisreset /restart >nul 2>&1
		timeout /t 2 /nobreak >nul
		IF ERRORLEVEL 1 echo IIS reset skipped or failed; likely requires elevated shell.
	)
)

where dotnet >nul 2>&1
IF NOT ERRORLEVEL 1 (
	echo Shutting down dotnet build servers to clear stale file locks...
	dotnet build-server shutdown >nul 2>&1
)

RMDIR /S /Q "Bin"
IF not defined NoRebuild (
	FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
	FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
	FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin_dotnet') DO RMDIR /S /Q "%%G"
)

IF not defined MsBuildSwitches ( Set MsBuildSwitches=/v:n /nr:false)
IF not defined FuseCPVersion ( Set FuseCPVersion=2.0.0)
IF not defined FuseCPFileVersion ( Set FuseCPFileVersion=2.0.0.0)
IF not defined Configuration ( Set Configuration=Release)
IF not defined BuildLegacyPackaging ( Set BuildLegacyPackaging=false)
IF not defined BuildLinuxInstallPackages ( Set BuildLinuxInstallPackages=false)

IF EXIST "%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=18.0
	Echo Found VS 2026 Community
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\18\Professional\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=18.0
	Echo Found VS 2026 Professional
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=18.0
	Echo Found VS 2026 Enterprise
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\18\Preview\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\18\Preview\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=18.0
	Echo Found VS 2026 Preview
	GOTO Build 
 )
 IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=18.0
	Echo Found VS 2026 Build Tools
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=17.0
	Echo Found VS 2022 Community
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=17.0
	Echo Found VS 2022 Professional
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=17.0
	Echo Found VS 2022 Enterprise
	GOTO Build 
 )
IF EXIST "%ProgramFiles%\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=17.0
	Echo Found VS 2022 Preview
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=16.0
	Echo Found VS 2019 Community
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=16.0
	Echo Found VS 2019 Professional
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
	Set FCPVSVer=16.0
	Echo Found VS 2019 Enterprise
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
	Set FCPVSVer=15.0
	Echo Found VS 2017 Community
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
	Set FCPVSVer=15.0
	Echo Found VS 2017 Professional
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
	Set FCPVSVer=15.0
	Echo Found VS 2017 Enterprise
	GOTO Build 
 )
IF EXIST "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" (
	Set FCPMSBuild="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
	Set FCPVSVer=14.0
	Echo Found VS 2015
	GOTO Build 
 )

echo "VisualStudio not found. VisualStudio must be installed to build FuseCP."

Set FCPMSBuild="msbuild"

:Build
dotnet msbuild build.xml /target:Deploy ^
	/p:BuildConfiguration=%Configuration% ^
	/p:Version="%FuseCPVersion%" ^
	/p:FileVersion="%FuseCPFileVersion%" ^
	/p:VersionLabel="%FuseCPFileVersion%" ^
	/p:BuildLegacyPackaging=%BuildLegacyPackaging% ^
	/p:BuildLinuxInstallPackages=%BuildLinuxInstallPackages% ^
	%MsBuildSwitches% ^
	/fileLogger ^
	/flp:verbosity=normal ^
	/p:VisualStudioVersion=%FCPVSVer%
set ERR=%ERRORLEVEL%
popd
endlocal & exit /b %ERR%
