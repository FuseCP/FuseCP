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
 * FuseCP WHMCS FuseCP / WHMCS Hooks
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/enterpriseserver.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/var_definition.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/database.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/settings.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/audit_logger.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/input_validator.php');

/**
 * Decrypt a server password using the WHMCS Capsule / localAPI helper.
 * Replaces the deprecated global decrypt() function removed in WHMCS 8.x.
 *
 * @param string $encrypted Encrypted password from tblservers.password
 * @return string Plaintext password
 */
function fusecp_decrypt_password(string $encrypted): string
{
    // WHMCS 8.x+: use the Crypt helper
    if (class_exists('\\WHMCS\\Crypt\\Hash')) {
        return \WHMCS\Crypt\Hash::decrypt($encrypted);
    }
    // WHMCS 7.x fallback: the global decrypt() function is still available
    if (function_exists('decrypt')) {
        return decrypt($encrypted);
    }
    // Last-resort: return as-is and log a warning
    logactivity('FuseCP hooks: could not decrypt server password – no decryption method available.', 0);
    return $encrypted;
}

/**
 * Handles updating FuseCP account details when a client or administrator updates a client's details
 * 
 * @access public
 * @param array $params WHMCS parameters
 * @throws Exception
 */
function fusecp_module_ClientEdit($params)
{
    $fusecp_settings = new fusecp_settings;
    $fusecp_settings->getSettings();
    if($fusecp_settings->settings['NeedFirstConfiguration'] == 1) return false;
    if($fusecp_settings->settings['NeedMigration'] == 1) return false;
    if($fusecp_settings->settings['SyncActive'] != 1) return false;

    // WHMCS server parameters & package parameters
    $userid = $params['userid'];
    $serviceid = 0;
    
    // Query for the users FuseCP accounts - If they do not have any, just ignore the request
    $fcpaccounts = fusecp_database::getUserFCPAccounts($userid);
    if(is_array($fcpaccounts) && $fcpaccounts['status']=='error'){
        throw new Exception($fcpaccounts['description']);
    }
    if(!empty($fcpaccounts)){
        foreach($fcpaccounts as $fcpaccount){
            // Start updating the users account details
            $serviceid = $fcpaccount->serviceid;
            $username = $fcpaccount->username;
            $serverUsername = $fcpaccount->serverusername;
            $serverPassword = fusecp_decrypt_password($fcpaccount->serverpassword);
            $serverPort = empty($fcpaccount->serverport) ? '9002' : $fcpaccount->serverport;
            $serverHost = empty($fcpaccount->serverhostname) ? $fcpaccount->serverip : $fcpaccount->serverhostname;
            $serverSecure = $fcpaccount->serversecure == 'on' ? TRUE : FALSE;
            $clientsDetails = $params;
            {
                // Create the FuseCP Enterprise Server Client object instance
                $fcp = new FuseCP_EnterpriseServer($serverUsername, $serverPassword, $serverHost, $serverPort, $serverSecure);

                // Get the user's details from FuseCP - We need the username
                $user = $fcp->getUserByUsername($username);
                if (empty($user))
                {
                    throw new Exception("User {$username} does not exist - Cannot update account details for unknown user");
                }

                // Update the user's account details using the previous details + WHMCS's details (address, city, state etc.)
                $userParams = array('RoleId' => $user['RoleId'],
                                'Role' => $user['Role'],
                                'StatusId' => $user['StatusId'],
                                'Status' => $user['Status'],
                                'LoginStatusId' => $user['LoginStatusId'],
                                'LoginStatus' => $user['LoginStatus'],
                                'FailedLogins' => $user['FailedLogins'],
                                'UserId' => $user['UserId'],
                                'OwnerId' => $user['OwnerId'],
                                'IsPeer' => $user['IsPeer'],
                                'Created' => $user['Created'],
                                'Changed' => $user['Changed'],
                                'IsDemo' => $user['IsDemo'],
                                'Comments' => $user['Comments'],
                                'LastName' => $clientsDetails['lastname'],
                                'Username' => $user['Username'],
                                'Password' => $user['Password'],
                                'FirstName' => $clientsDetails['firstname'],
                                'Email' => $clientsDetails['email'],
                                'PrimaryPhone' => $clientsDetails['phonenumber'],
                                'Zip' => $clientsDetails['postcode'],
                                'InstantMessenger' => '',
                                'Fax' => '',
                                'SecondaryPhone' => '',
                                'SecondaryEmail' => '',
                                'Country' => $clientsDetails['country'],
                                'Address' => $clientsDetails['address1'],
                                'City' => $clientsDetails['city'],
                                'State' => $clientsDetails['state'],
                                'HtmlMail' => $user['HtmlMail'],
                                'CompanyName' => $clientsDetails['companyname'],
                                'EcommerceEnabled' => $user['EcommerceEnabled'],
                                'SubscriberNumber' => '',
								'MfaMode' => $user['MfaMode']);

                // Execute the UpdateUserDetails method
                $fcp->updateUserDetails($userParams);

                // Add log entry to client log
                logactivity("FuseCP Sync - Account {$username} contact details updated successfully", $userid);
                FuseCP_AuditLogger::success('SYNC_CLIENT_EDIT', $userid, "Account {$username} contact details updated", ['service_id' => $serviceid]);
            }
            catch (Exception $e)
            {
                // Error message to log / return
                $errorMessage = "fusecp_module_ClientEdit Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}, Service ID: {$serviceid})";

                // Log to WHMCS
                logactivity($errorMessage, $userid);
                FuseCP_AuditLogger::failure('SYNC_CLIENT_EDIT', $userid, $errorMessage, ['service_id' => $serviceid]);
            }
        }
    }
}

/**
 * Handles activating and adding client addons to FuseCP
 * 
 * @access public
 * @param array $params WHMCS parameters
 * @throws Exception
 */
function fusecp_module_AddonActivation($params)
{
    $fusecp_settings = new fusecp_settings;
    $fusecp_settings->getSettings();
    if($fusecp_settings->settings['NeedFirstConfiguration'] == 1) return false;
    if($fusecp_settings->settings['NeedMigration'] == 1) return false;
    if($fusecp_settings->settings['AddonsActive'] != 1) return false;

    // WHMCS server parameters & package parameters
    $userid = $params['userid'];
    $serviceid = $params['serviceid'];
    $addonid = $params['addonid'];

    try
    {
        $fcpaccount = fusecp_database::getAddonActivationFCPAccount($serviceid, $addonid);
        if(is_array($fcpaccount) && $fcpaccount['status']=='error'){
            throw new Exception($fcpaccount['description']);
        }

        if (!empty($fcpaccount)){
            // Start processing the users addon
            $username = $fcpaccount->username;
            $serverUsername = $fcpaccount->serverusername;
            $serverPassword = fusecp_decrypt_password($fcpaccount->serverpassword);
            $serverPort = empty($fcpaccount->serverport) ? '9002' : $fcpaccount->serverport;
            $serverHost = empty($fcpaccount->serverhostname) ? $fcpaccount->serverip : $fcpaccount->serverhostname;
            $serverSecure = $fcpaccount->serversecure == 'on' ? TRUE : FALSE;
        
            // Create the FuseCP Enterprise Server Client object instance
            $fcp = new FuseCP_EnterpriseServer($serverUsername, $serverPassword, $serverHost, $serverPort, $serverSecure);
        
            // Get the user's details from FuseCP - We need the userid
            $user = $fcp->getUserByUsername($username);
            if (empty($user))
            {
                throw new Exception("User {$username} does not exist - Cannot allocate addon for unknown user");
            }
            
            // Get the user's package details from FuseCP - We need the PackageId
            $package = $fcp->getUserPackages($user['UserId']);
            $packageId = $package['PackageId'];
            
            // Get the associated FuseCP addon id
            $addon = fusecp_database::getFCPAddon($addonid);
            if(is_array($addon) && $addon['status']=='error'){
                throw new Exception($addon['description']);
            }
            elseif(empty($addon)){
                throw new Exception("WHMCS AddonID {$addonid} doesn't exists in".SOLIDCP_ADDONS_TABLE);
            }
            
            // Add the Addon Plan to the customer's FuseCP package / hosting space
            $results = $fcp->addPackageAddonById($packageId, $addon->fcp_id);
            
            // Check the results to verify that the addon has been successfully allocated
            if ($results['Result'] > 0)
            {
                // If this addon is an IP address addon - attempt to randomly allocate an IP address to the customer's hosting space
                if ($addon->is_ipaddress == 1)
                {
                    $fcp->allocatePackageIPAddresses($packageId);
                }
                
                // Add log entry to client log
                logactivity("FuseCP Addon - Account {$username} addon successfully completed - Addon ID: {$addonid}", $userid);
                FuseCP_AuditLogger::success('ADDON_ACTIVATION', $userid, "Addon {$addonid} activated for account {$username}", ['service_id' => $serviceid]);
            }
            else
            {
                // Add log entry to client log
                throw new Exception("Unknown", $results['Result']);
            }
        }
    }
    catch (Exception $e)
    {
        // Error message to log / return
        $errorMessage = "fusecp_addons_AddonActivation Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}, Service ID: {$serviceid})";

        // Log to WHMCS
        logactivity($errorMessage, $userid);
        FuseCP_AuditLogger::failure('ADDON_ACTIVATION', $userid, $errorMessage, ['service_id' => $serviceid]);
    }
}

/**
 * Handles deleting addons to FuseCP from Admin area
 * 
 * @access public
 * @param array $params WHMCS parameters
 * @throws Exception
 */
/*  FIX ME!!!!!!
 * This code is executed, before the values are saved to the WHMCS DB. Therefore "modulechangepackage" doesn't work.
 * 
function fusecp_module_AddonDeleted($params)
{
    $fusecp_settings = new fusecp_settings;
    $fusecp_settings->getSettings();
    if($fusecp_settings->settings['NeedFirstConfiguration'] == 1) return false;
    if($fusecp_settings->settings['NeedMigration'] == 1) return false;
    if($fusecp_settings->settings['AddonsActive'] != 1) return false;

    $id = $params['id'];
    $result = full_query("SELECT h.id AS serviceid FROM `tblhostingaddons` AS a, `tblhosting` AS h, `tblservers` AS s, `tblproducts` AS p WHERE a.id = {$id} AND h.id = a.hostingid AND h.packageid = p.id AND h.server = s.id AND s.type = 'FuseCP' AND h.domainstatus IN ('Active', 'Suspended')");
    if (mysql_num_rows($result) > 0)
    {
        $adminuser = $fusecp_settings->settings['WhmcsAdmin'];
        $row = mysql_fetch_assoc($result);
        $values["serviceid"]  = $row['serviceid'];
        $command = "modulechangepackage";
        $results = localAPI($command,$values,$adminuser);
        if($results['result'] == 'success') logactivity("FuseCP Addon - Addon ID: {$id} successfully deleted.");
        elseif($results['result'] == 'error') logactivity("FuseCP Addon - Addon ID: {$id} couldn't be deleted. Error: {$results['message']}");
    }
}*/

/* Update Client Contact Details - FuseCP */
add_hook('ClientEdit', 1, 'fusecp_module_ClientEdit');
/* Addon Activation/Deleting - FuseCP */
add_hook('AddonActivation', 1, 'fusecp_module_AddonActivation');
add_hook('AddonAdd', 1, 'fusecp_module_AddonActivation');
//add_hook('AddonDeleted', 1, 'fusecp_module_AddonDeleted');

/**
 * Hook: Log failed client login attempts for FuseCP account awareness.
 * Records failed logins in the FuseCP audit log so administrators can
 * correlate WHMCS login failures with FuseCP access patterns.
 *
 * @param array $params WHMCS hook parameters
 */
function fusecp_module_ClientLoginFailed(array $params): void
{
    $userId = (int)($params['userid'] ?? 0);
    $email  = $params['email'] ?? $params['username'] ?? '';

    FuseCP_AuditLogger::failure(
        'CLIENT_LOGIN_FAILED',
        $userId,
        "Failed login attempt for account: {$email}",
        ['ip_address' => $_SERVER['REMOTE_ADDR'] ?? '']
    );
}

add_hook('ClientLoginFailed', 1, 'fusecp_module_ClientLoginFailed');

