# Server Auth Recovery

This document defines the supported recovery path for Enterprise-to-Server credentials after the direct Portal and web API reset path was disabled.

## Portal behavior

The Server edit page Connection Settings section is URL-only. It does not rotate or store a new server credential.

The Server Tools card shows the Enterprise-side credential posture:

- whether Enterprise currently has a stored server credential
- whether Enterprise expects SHA-256 or legacy SHA-1 for that server

## New auth requirements

The current Enterprise-to-Server auth model has two layers:

1. Shared secret compatibility
   Enterprise stores an encrypted copy of the server credential in the `Servers.Password` column and tracks the hash mode in `Servers.PasswordIsSHA256`.

2. HMAC request signing
   Modern servers advertise support through AutoDiscovery and then accept signed requests using:
   - version header `X-FuseCP-Server-Auth-Version: 2`
   - timestamp header
   - nonce header
   - key id header
   - optional cluster id header
   - HMAC-SHA256 signature header

For the modern path to work cleanly, all of the following must be true:

- the server host password was reset to the intended secret on the server host
- Enterprise stores the same clear-text secret, encrypted with the Enterprise crypto key
- `PasswordIsSHA256` is `true` for modern SHA-256-based server credentials
- AutoDiscovery reports `SupportsHmacAuthentication = true`
- AutoDiscovery reports `PasswordIsSha256 = true`
- the server and Enterprise clocks are within the allowed skew window

Legacy fallback still exists for mixed-version environments, but it is a compatibility path, not the target posture.

## Runtime config persistence

Modern server-side hardening writes the effective authentication override to `appsettings.hardened.json`, not to `appsettings.json` or `bin_dotnet\appsettings.json`.

This separation is intentional:

- `appsettings.json` remains installer-managed and can be replaced by build or deployment steps
- `appsettings.hardened.json` is a runtime overlay loaded after `appsettings.json`, so it overrides only the hardened values that must change at runtime
- the overlay contains only the server authentication values written by hardening: `Server.Password` and `Server.AllowLegacyPasswordAuthentication`
- the overlay file should be writable by the Server IIS application pool identity, but executable folders and the main config files should remain non-writable where possible

For correctly installed or updated setups, the installer pre-creates `appsettings.hardened.json` and applies the normal server-site file permissions to it.

For already installed setups that have not yet been updated with the newer installer behavior, create the file once and grant Modify on that file only:

```powershell
if (-not (Test-Path "C:\FuseCP\Server\appsettings.hardened.json")) {
   '{}' | Set-Content "C:\FuseCP\Server\appsettings.hardened.json" -Encoding UTF8
}

icacls "C:\FuseCP\Server\appsettings.hardened.json" /grant "IIS AppPool\FuseCP Server:(M)"
```

Do not grant the app pool write access to `bin_dotnet`, DLL folders, or the main `appsettings.json` unless a temporary local-development workaround is absolutely required.

## Supported recovery flow

Use this sequence when a server credential must be recovered or rotated.

1. Reset the server-host credential on the server host.

Supported host-side reset methods:

- `FuseCP-Configuration.ps1`
- `FuseCP.SilentInstaller.exe /cname:"Server" /passw:"<secret>" ...`

Those flows update the Server-side configuration. They do not provide a supported Portal-based takeover path.

2. Reconcile the Enterprise-side stored credential.

Use the emergency CLI wrapper from the repository root:

```powershell
pwsh -File FuseCP/Tools/Recover-ServerCredential.ps1 -ServerId 5 -Password "<secret>" -Mode sha256
```

Options:

- `-ServerId` or `-ServerName`: identify the existing server record to update
- `-Password`: the same secret that was set on the server host
- `-Mode keep`: preserve the current Enterprise `PasswordIsSHA256` flag
- `-Mode sha256`: set Enterprise to modern SHA-256 mode
- `-Mode sha1`: preserve or restore legacy SHA-1 mode for older servers only
- `-DryRun`: validate target resolution without writing changes
- `-ConfigPath`: point at a specific EnterpriseServer `Web.config` if the default path is not being used

3. Validate the result.

- confirm the server can be contacted from the Portal
- confirm the Server Tools card shows the expected credential state
- confirm the Password Lifecycle dashboard reports the expected auth posture

## What is not supported

The following are intentionally not supported recovery paths:

- direct Portal password reset
- direct Enterprise web API password reset
- using standalone provisioning flows that call `AddServer(...)` as a routine recovery mechanism for an already-registered server

Standalone installer registration paths are provisioning flows. For an existing server, use host-side reset plus Enterprise reconciliation instead of creating or re-adding a server record.