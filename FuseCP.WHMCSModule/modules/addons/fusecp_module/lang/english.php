<?php if (!defined('WHMCS')) exit('ACCESS DENIED');
// Copyright (c) 2023, FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license
// 
// FuseCP is a fork of WebsitePanel:
// Copyright (c) 2015, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


/**
 * FuseCP WHMCS Server Module Client Area Language
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */
$_ADDONLANG['FuseCP_migration'] = 'Migration';
$_ADDONLANG['FuseCP_settings'] = 'Settings';
$_ADDONLANG['FuseCP_addon_automation'] = 'Addon Automation';
$_ADDONLANG['FuseCP_configurable_options'] = 'Configurable Options';
$_ADDONLANG['FuseCP_sync_automation'] = 'Sync Automation';
$_ADDONLANG['FuseCP_close'] = 'Close';
$_ADDONLANG['FuseCP_yes'] = 'Yes';
$_ADDONLANG['FuseCP_no'] = 'No';
$_ADDONLANG['FuseCP_action'] = 'Action';
$_ADDONLANG['FuseCP_checkagain'] = 'Check again!';
$_ADDONLANG['FuseCP_addonsnotactive'] = 'Addon automation is not active. Please activate it on the "Setting" page first.';
$_ADDONLANG['FuseCP_configurablenotactive'] = 'Configurable options automation is not active. Please activate it on the "Setting" page first.';
$_ADDONLANG['FuseCP_syncnotactive'] = 'Sync automation is not active. Please activate it on the "Setting" page first.';
$_ADDONLANG['FuseCP_sync_nosetting'] = 'Sync automation has no settings right now';

$_ADDONLANG['FuseCP_migration_needed'] = 'A migration is needed, before you can continue to use this module!';
$_ADDONLANG['FuseCP_migrateTable_text'] = 'Copy database table content from "%s" to "%s".';
$_ADDONLANG['FuseCP_migrateDbValues_text'] = 'Value "%s" from table "%s" need to be migrated due to structure changes.';
$_ADDONLANG['FuseCP_deactivateModules_text'] = 'The module "%s" needs to be manually deactivated after the previous steps are done!';
$_ADDONLANG['FuseCP_deleteFiles_text'] = 'Delete file "%s" manually via FTP after all previous steps are done!';
$_ADDONLANG['FuseCP_confirmdelete'] = 'Confirm deletion';
$_ADDONLANG['FuseCP_confirmdelete_long'] = 'Are you sure, that you want to delete this table? All values inside this table will be deleted. This action is irreversible after your confirmation!';
$_ADDONLANG['FuseCP_copytable'] = 'Copy old table "%s" to new "%s"';
$_ADDONLANG['FuseCP_deletetable'] = 'Delete old "%s" table (not recommended)';
$_ADDONLANG['FuseCP_migratedbvalues'] = 'Migrate values in table "%s" to new structure';
$_ADDONLANG['FuseCP_deactivatemodule'] = 'Goto "System" -> "Addon Module"';
$_ADDONLANG['FuseCP_confirmmigrate'] = 'Confirm migration';
$_ADDONLANG['FuseCP_confirmmigrate_long'] = 'Are you sure, that you want to migrate this database table? There is no way back to use the old module anymore. Please create a backup of this table before strarting the migration. This action is irreversible after your confirmation!';
$_ADDONLANG['FuseCP_migrationrunning'] = 'Migration task is running';
$_ADDONLANG['FuseCP_saverunning'] = 'Save task is running';
$_ADDONLANG['FuseCP_needfirstconfiguration'] = 'This is the first module configuration. Please adjust and save your settings!';
$_ADDONLANG['FuseCP_norecordsfound'] = 'No records found';

$_ADDONLANG['FuseCP_configurableoptionname'] = 'Configurable Option Name';
$_ADDONLANG['FuseCP_addonname'] = 'Addon Name';
$_ADDONLANG['FuseCP_whmcs_id'] = 'WHMCS ID';
$_ADDONLANG['FuseCP_fusecp_id'] = 'FuseCP ID';
$_ADDONLANG['FuseCP_hidden'] = 'Hidden';
$_ADDONLANG['FuseCP_delete'] = 'Delete';
$_ADDONLANG['FuseCP_edit_configurable_option'] = 'Edit configurable option assignment';
$_ADDONLANG['FuseCP_edit_addon_option'] = 'Edit addon assignment';
$_ADDONLANG['FuseCP_whmcs_id_tooltip'] = 'Fill the WHMCS-ID from the database.';
$_ADDONLANG['FuseCP_fusecp_id_tooltip'] = 'Fill the FuseCP Addon-ID';
$_ADDONLANG['FuseCP_is_ip_address'] = 'Is IP address';
$_ADDONLANG['FuseCP_is_ip_address_tooltip'] = 'Should a new dedicated IP address be assigned in FuseCP?';
$_ADDONLANG['FuseCP_search_configurable'] = 'Search for configurable name or FuseCP-ID...';
$_ADDONLANG['FuseCP_show_only_assigned_conf'] = 'Show only assigned configurable';
$_ADDONLANG['FuseCP_search_addon'] = 'Search for addon name or FuseCP-ID...';
$_ADDONLANG['FuseCP_show_only_assigned_addons'] = 'Show only assigned addons';

$_ADDONLANG['FuseCP_general_settings'] = 'General Settings';
$_ADDONLANG['FuseCP_setting_AddonsActive'] = 'Addon Automation active';
$_ADDONLANG['FuseCP_setting_AddonsActive_tooltip'] = 'Addon provisioning will be automated if this option is checked. Add entries to the Addon Automation tab in order to get it working.';
$_ADDONLANG['FuseCP_setting_ConfigurableOptionsActive'] = 'Configurable Options active';
$_ADDONLANG['FuseCP_setting_ConfigurableOptionsActive_tooltip'] = 'Configurable options provisioning will be automated if this option is checked and a valid license is found. Add entries to the Configurable Options tab in order to get it working.';
$_ADDONLANG['FuseCP_setting_SyncActive'] = 'Sync Automation active';
$_ADDONLANG['FuseCP_setting_SyncActive_tooltip'] = 'Client details will be synced automatically to FuseCP account if changes in WHMCS are made.';
$_ADDONLANG['FuseCP_setting_DeleteTablesOnDeactivate'] = 'Delete DB tables on deactivation';
$_ADDONLANG['FuseCP_setting_DeleteTablesOnDeactivate_tooltip'] = 'Database tables will be deleted, if module deactivation will be performed and this option is checked.';
$_ADDONLANG['FuseCP_setting_WhmcsAdmin'] = 'WHMCS admin for API calls';
$_ADDONLANG['FuseCP_setting_WhmcsAdmin_tooltip'] = 'WHMCS admin user, who will be used for internal API calls from client area scripts (e.g. for changing product password).';

$_ADDONLANG['FuseCP_save_changes'] = 'Save Changes';
$_ADDONLANG['FuseCP_cancel_changes'] = 'Cancel Changes';
$_ADDONLANG['FuseCP_edit'] = 'Edit';
$_ADDONLANG['FuseCP_cancel'] = 'Cancel';
$_ADDONLANG['FuseCP_saving'] = 'Saving in progress';

// v2.0.0 additions
$_ADDONLANG['FuseCP_audit_log'] = 'Audit Log';
$_ADDONLANG['FuseCP_audit_log_action'] = 'Action';
$_ADDONLANG['FuseCP_audit_log_status'] = 'Status';
$_ADDONLANG['FuseCP_audit_log_detail'] = 'Detail';
$_ADDONLANG['FuseCP_audit_log_user'] = 'User ID';
$_ADDONLANG['FuseCP_audit_log_date'] = 'Date';
$_ADDONLANG['FuseCP_audit_log_ip'] = 'IP Address';
$_ADDONLANG['FuseCP_audit_log_service'] = 'Service ID';
$_ADDONLANG['FuseCP_audit_log_duration'] = 'Duration (ms)';
$_ADDONLANG['FuseCP_security_settings'] = 'Security Settings';
$_ADDONLANG['FuseCP_setting_EnforceHttps'] = 'Enforce HTTPS for Enterprise Server';
$_ADDONLANG['FuseCP_setting_EnforceHttps_tooltip'] = 'When enabled, connections to the FuseCP Enterprise Server will always use HTTPS. Recommended for production environments.';
$_ADDONLANG['FuseCP_setting_VerifySslCert'] = 'Verify SSL certificate';
$_ADDONLANG['FuseCP_setting_VerifySslCert_tooltip'] = 'Validate the TLS certificate of the FuseCP Enterprise Server. Disable only in development/test environments.';
$_ADDONLANG['FuseCP_api_credentials'] = 'API Credentials';
$_ADDONLANG['FuseCP_api_key_id'] = 'API Key ID';
$_ADDONLANG['FuseCP_api_key_secret'] = 'API Key Secret';
$_ADDONLANG['FuseCP_api_auth_method'] = 'Authentication Method';
$_ADDONLANG['FuseCP_api_auth_hmac'] = 'HMAC-SHA256 (Recommended)';
$_ADDONLANG['FuseCP_api_auth_soap'] = 'SOAP (Legacy – Deprecated)';
$_ADDONLANG['FuseCP_error_retry_enabled'] = 'Enable retry on transient errors';
$_ADDONLANG['FuseCP_error_retry_enabled_tooltip'] = 'Automatically retry failed API calls using exponential back-off. Recommended for production environments.';
$_ADDONLANG['FuseCP_error_max_retries'] = 'Maximum retries';
$_ADDONLANG['FuseCP_error_max_retries_tooltip'] = 'Number of retry attempts before a provisioning error is reported (1–5).';
$_ADDONLANG['FuseCP_soap_deprecation_notice'] = 'Warning: SOAP authentication is deprecated and will be removed in a future release. Please migrate to HMAC-SHA256.';
$_ADDONLANG['FuseCP_version'] = 'Module Version';
$_ADDONLANG['FuseCP_compatibility'] = 'WHMCS 7.x, 8.x, 9.x | PHP 7.4, 8.0, 8.1, 8.2';
