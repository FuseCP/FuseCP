cd .\Fedora
$size = ((ls -r|measure -s Length).Sum / 1024).ToString("f0")
$version = "$($args[0])".Trim()
if ([string]::IsNullOrWhiteSpace($version)) {
	throw "ConfigureFedora.ps1 requires a non-empty version argument."
}
$spec = Get-Content .\SPECS\fusecp.spec -Raw
$spec = [Regex]::Replace(
	$spec,
	"(?m)^(Version:\s*)[^\r\n]*$",
	[System.Text.RegularExpressions.MatchEvaluator]{
		param($m)
		return "$($m.Groups[1].Value)$version"
	}
)
[Environment]::CurrentDirectory = (Get-Location).Path
$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
[IO.File]::WriteAllText(".\SPECS\fusecp.spec", $spec, $Utf8NoBomEncoding)
cd ..
