# FuseCP WHMCS Module — Migration Guide v1.1.4 → v2.0.0

## Overview

Version 2.0.0 is a security and compatibility overhaul of the FuseCP WHMCS module. It is fully backward compatible with v1.1.4 configuration. Existing SOAP-based server configurations continue to work without any changes.

---

## What's New in v2.0.0

| Area | Change |
|------|--------|
| Security | Removed `phpinfo()` debug pages (`index.php`, `test.php`, `phpinfo.php`) |
| Security | Added `.htaccess` protection to module directories |
| Security | Fixed SQL injection vulnerabilities in raw database queries |
| Security | Replaced deprecated `decrypt()` with WHMCS-version-aware wrapper |
| Security | Added SSL peer-certificate verification to SOAP client |
| Feature | New `fusecp_audit_log` database table for API call auditing |
| Feature | Centralized audit logging (`audit_logger.php`) |
| Feature | Retry logic with exponential back-off (`error_handler.php`) |
| Feature | Input validation library (`input_validator.php`) |
| Feature | REST API client stub with HMAC-SHA256 auth (`rest_api_client.php`) |
| Feature | New `ClientLoginFailed` WHMCS hook |
| Compat | WHMCS 9.x and PHP 8.2 compatibility |

---

## Before You Start

1. **Create a full backup** of your WHMCS database and all module files.
2. Test the migration on a staging environment before applying it to production.
3. Ensure your server is running PHP 7.4 or later and WHMCS 7.x or later.

---

## Migration Steps

### Step 1: Back up your database

```sql
mysqldump -u whmcs_user -p whmcs_database > whmcs_backup_before_v2.sql
```

### Step 2: Upload the new module files

Unpack the v2.0.0 zip archive and upload all files to your WHMCS root directory, overwriting existing files.

### Step 3: Re-activate the module (to create new database tables)

In the WHMCS Admin Panel:
1. Go to **System Settings → Addon Modules**
2. Deactivate `FuseCP Module` (choose *do not delete tables* when prompted)
3. Reactivate `FuseCP Module`

The activation process will automatically create the new `fusecp_audit_log` table.

Alternatively, the audit log table will be created lazily the first time the module is used.

### Step 4: Verify your server configurations

All existing server configurations (hostname, port, username, password) remain unchanged. The module continues to use SOAP authentication by default.

To verify connectivity, go to **System Settings → Servers**, select your FuseCP server, and click **Test Connection**.

### Step 5: Review security settings (optional but recommended)

If your FuseCP Enterprise Server is accessed over HTTPS and has a valid TLS certificate, you can enable SSL certificate verification:

- In the module settings or server configuration, set `verifySsl = TRUE` (this is the default for new installs).

---

## Emergency Recovery Procedures

### Lost MFA Device (End User)

If a client has lost their MFA/2FA device and cannot log in to WHMCS:

1. As an administrator, go to **Clients → [Client Name] → Security**
2. Click **Disable Two-Factor Authentication** to temporarily disable MFA
3. The client can then log in and re-enrol a new MFA device

### Lost FuseCP Server Admin Password

If the FuseCP server administrator password has been lost:

1. Log in directly to the FuseCP server machine
2. Use the FuseCP command-line tool or database to reset the `serveradmin` password
3. Update the server password in WHMCS: **System Settings → Servers → [Your FuseCP Server] → Edit → Password**
4. Click **Save Changes**

### WHMCS Server Password Stored Encrypted

The server passwords stored in WHMCS `tblservers.password` are encrypted with WHMCS's internal encryption. If you need to verify the stored password, use WHMCS's built-in decryption (available to admin users via localAPI or the Crypt utility).

### Module Not Connecting to FuseCP After Upgrade

1. Check that the FuseCP Enterprise Server service is running on the configured host/port
2. Verify network connectivity: `telnet <host> <port>` or `curl https://<host>:<port>/esTest?WSDL`
3. Check WHMCS activity log for specific error messages
4. Review the `fusecp_audit_log` table for recent failure entries:
   ```sql
   SELECT * FROM fusecp_audit_log WHERE status = 'error' ORDER BY created_at DESC LIMIT 20;
   ```

### Rollback to v1.1.4

If you need to roll back:

1. Restore the module files from your backup
2. Restore the WHMCS database from your backup (or drop the `fusecp_audit_log` table manually)
3. Reactivate the module in WHMCS

The `fusecp_settings`, `fusecp_addons`, and `fusecp_configurable` tables are unchanged in v2.0.0 and do not need to be migrated back.

---

## Database Changes

### New Table: `fusecp_audit_log`

```sql
CREATE TABLE `fusecp_audit_log` (
  `id`          INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  `action`      VARCHAR(100) NOT NULL,
  `userid`      INT NOT NULL DEFAULT 0,
  `status`      VARCHAR(20) NOT NULL,
  `detail`      VARCHAR(500) NULL,
  `api_method`  VARCHAR(100) NULL,
  `service_id`  INT NOT NULL DEFAULT 0,
  `duration_ms` INT NOT NULL DEFAULT 0,
  `ip_address`  VARCHAR(45) NULL,
  `created_at`  DATETIME NULL,
  `updated_at`  DATETIME NULL,
  INDEX `idx_action` (`action`),
  INDEX `idx_userid` (`userid`)
) ENGINE=InnoDB;
```

This table is created automatically on module activation. If the table does not exist, audit entries are written only to the WHMCS activity log (graceful degradation).

---

## Frequently Asked Questions

**Q: Do I need to change my WHMCS server product configurations?**
A: No. All existing configurations continue to work without changes.

**Q: Does the new version still support SOAP authentication?**
A: Yes. SOAP authentication remains the default and is fully supported. The new `rest_api_client.php` provides a REST/HMAC client for future use when FuseCP Enterprise Server v2+ REST endpoints are available.

**Q: What happened to `index.php`, `test.php`, and `phpinfo.php`?**
A: These files contained `phpinfo()` output and debug code that exposed sensitive server information. They have been replaced with secure access-denied stubs.

**Q: Will the module work with WHMCS 9.x?**
A: Yes. Version 2.0.0 has been tested with WHMCS 7.x, 8.x, and 9.x.

**Q: Will the module work with PHP 8.2?**
A: Yes. The deprecated `decrypt()` function has been replaced with a version-aware wrapper.
