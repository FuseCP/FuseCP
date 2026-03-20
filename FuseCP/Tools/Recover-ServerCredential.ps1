param(
    [string]$ConfigPath,
    [int]$ServerId,
    [string]$ServerName,
    [Parameter(Mandatory = $true)]
    [string]$Password,
    [ValidateSet('keep', 'sha256', 'sha1')]
    [string]$Mode = 'keep',
    [switch]$DryRun
)

$projectPath = Join-Path $PSScriptRoot '..\Sources\Tools\FuseCP.ServerCredentialRecovery.Cli\FuseCP.ServerCredentialRecovery.Cli.csproj'
$projectPath = [System.IO.Path]::GetFullPath($projectPath)

if (-not (Test-Path $projectPath)) {
    throw "Recovery CLI project was not found at $projectPath"
}

if (($PSBoundParameters.ContainsKey('ServerId') -and $PSBoundParameters.ContainsKey('ServerName')) -or
    (-not $PSBoundParameters.ContainsKey('ServerId') -and -not $PSBoundParameters.ContainsKey('ServerName'))) {
    throw 'Specify exactly one of -ServerId or -ServerName.'
}

$arguments = @('run', '--project', $projectPath, '--configuration', 'Release', '--')

if ($ConfigPath) {
    $arguments += @('--config', $ConfigPath)
}

if ($PSBoundParameters.ContainsKey('ServerId')) {
    $arguments += @('--server-id', $ServerId)
}
else {
    $arguments += @('--server-name', $ServerName)
}

$arguments += @('--password', $Password, '--mode', $Mode)

if ($DryRun) {
    $arguments += '--dry-run'
}

& dotnet @arguments
exit $LASTEXITCODE