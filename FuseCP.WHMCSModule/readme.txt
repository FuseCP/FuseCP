Supported WHMCS version: 7.x, 8.x, 9.x (versions <= 6.x are not supported)
Supported PHP version: 7.4, 8.0, 8.1, 8.2 (PHP < 7.4 is NOT supported)

PHP/WHMCS Compatibility Matrix:
+----------+---------+---------+---------+
| Module   | WHMCS 7 | WHMCS 8 | WHMCS 9 |
+----------+---------+---------+---------+
| v2.0.0   |   ✓     |   ✓     |   ✓     |
| v1.1.4   |   ✓     |   ✓     |   -     |
+----------+---------+---------+---------+

PHP Version Matrix:
+----------+---------+---------+---------+---------+
| Module   | PHP 7.4 | PHP 8.0 | PHP 8.1 | PHP 8.2 |
+----------+---------+---------+---------+---------+
| v2.0.0   |   ✓     |   ✓     |   ✓     |   ✓     |
| v1.1.4   |   ✓     |   ✓     |   ✓     |   -     |
+----------+---------+---------+---------+---------+

Installation instruction:
- Unpack the zip file
- Upload to your WHMCS root directory, overwrite existing files
- In Admin Panel go to "System" -> "Addon Modules"
- Activate "FuseCP Module"
- Configure "FuseCP Module" and grant access to Full Administrator -> Save changes
- In the main menu go to "Addons" -> "FuseCP Module"
- Adjust your settings.
- Add a new server to "System" -> "Products/Services" -> "Servers"
- Configure your products/services with the "FuseCP" module in Module Settings tab.
- Done!

Update instruction (from v1.1.4):
Please see MIGRATION_GUIDE_v2.0.0.md for full migration instructions.
- Make a full backup of your entire WHMCS database.
- Unpack the zip file
- Upload to your WHMCS root directory, overwrite existing files
- In Admin Panel go to "System" -> "Addon Modules"
- Activate "FuseCP Module"
- Configure "FuseCP Module" and grant access to "Full Administrator"
- In the main menu go to "Addons" -> "FuseCP Module"
- Follow migration instructions.
- When migration is finished, the Settings page will be shown automatically.
- Adjust your settings and check if Addons and Configurable Options were migrated successfully (when used in previous version)
- Done!

v. 2.0.0 - 18 March 2026
See changelog.txt for full details.
