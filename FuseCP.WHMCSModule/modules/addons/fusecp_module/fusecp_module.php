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
 * FuseCP Enterprise Server Client
 * 
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/configurableoptions.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/database.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/migration.php');
require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/settings.php');

/**
 * fusecp_module_config
 *
 * @access public
 * @return array
 */
function fusecp_module_config()
{
    return array('name' => 'FuseCP Module',
                 'description' => 'FuseCP Module for automating product configurable options, addons and sync to FuseCP',
                 'version' => '2.0.0',
                 'author' => '<a href="https://fusecp.com/" target="_blank">FuseCP</a>',
                 'language' => 'english');
}

/**
 * fusecp_module_activate
 *
 * @access public
 * @return array
 */
function fusecp_module_activate()
{
    // Create the FuseCP Module settings table
    $e = fusecp_database::createSettingsTable();
    if($e['status']!='success') return $e;

    // Create the FuseCP Addon table
    $e = fusecp_database::createAddonsTable();
    if($e['status']!='success') return $e;
    
    // Create the FuseCP Configurable Options table
    $e = fusecp_database::createConfigurableOptionsTable();
    if($e['status']!='success') return $e;

    // Create the FuseCP Audit Log table (v2.0.0+)
    $e = fusecp_database::createAuditLogTable();
    if($e['status']!='success') return $e;
    
    return array('status' => 'success', 'description' => 'The module has been activated successfully');
}

/**
 * fusecp_module_deactivate
 *
 * @access public
 * @return array
 */
function fusecp_module_deactivate()
{
    $fusecp_settings = new fusecp_settings;
    $fusecp_settings->getSettings();
    if($fusecp_settings->settings['DeleteTablesOnDeactivate'] == 1){

        // Delete the FuseCP Module settings table
        $e = fusecp_database::deleteSettingsTable();
        if($e['status']!='success') return $e;

        // Delete the FuseCP Addon table
        $e = fusecp_database::deleteAddonsTable();
        if($e['status']!='success') return $e;

        // Delete the FuseCP Configurable Options table
        $e = fusecp_database::deleteConfigurableOptionsTable();
        if($e['status']!='success') return $e;

        // Delete the FuseCP Audit Log table (v2.0.0+)
        $e = fusecp_database::deleteAuditLogTable();
        if($e['status']!='success') return $e;
        
        return array('status' => 'success', 'description' => 'The module has been deactivated and the tables have been deleted successfully');
    }

    return array('status' => 'success', 'description' => 'The module has been deactivated successfully. Tables were NOT deleted.');
}

/**
 * fusecp_moduleupgrade
 *
 * @param $vars array
 * @access public
 * @return array
 */
function fusecp_module_upgrade($vars)
{
    // Module versions
    $version = $vars['version'];

}

/**
 * Displays the FuseCP configurable module output
 *
 * @access public
 * @return mixed
 */
function fusecp_module_output($params)
{   
    define('DS', DIRECTORY_SEPARATOR);
    
    global $aInt, $templates_compiledir;
    $template = NULL;
    $fcp_smarty = new Smarty;
    $fcp_smarty->caching = false;
    $fcp_smarty->compile_dir = $templates_compiledir; 
    $fcp_smarty->assign('LANG',$params['_lang']);
    $fcp_smarty->assign('params',$params);

    if($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="migration"){
        $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
        $result = startMigration($_POST['command'],$_POST['value1'],$_POST['value2'],$_POST['option']);
        $html_ids = getHTMLids($_POST['command'],$_POST['value1'],$_POST['value2']);
        $fcp_smarty->assign('html_ids',$html_ids);
        $template = "ajax".DS."migration.tpl";
    }
    elseif($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="save_settings"){
        $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
        $fusecp_settings = new fusecp_settings;
        $fusecp_settings->getSettings();
        $result = $fusecp_settings->setSettings($_POST);
        $template = "ajax".DS."savesettings.tpl";
    }
    else{
        checkMigration();
        $fusecp_settings = new fusecp_settings;
        $result = $fusecp_settings->getSettings();
        $fcp_smarty->assign('admins',$fusecp_settings->admins);
        $fcp_smarty->assign('settings',$fusecp_settings->settings);
        if($fusecp_settings->settings['ConfigurableOptionsActive'] == 1){
            $fusecp_configurable = new fusecp_configurableoptions();
        }
        if($fusecp_settings->settings['AddonsActive'] == 1){
            $fusecp_addon = new fusecp_addonautomation();
        }
        if($fusecp_settings->settings['NeedMigration']){
                $fcp_smarty->assign('migrationsteps',migrationSteps());
        }
        if($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="load") {
            $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
            $template = "admin_".str_replace("#", "", strip_tags ($_POST['area'])).".tpl";
        }
        elseif($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="edit_configurable") {
            $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
            $result = $fusecp_configurable->setConfigurableOption($_POST);
			$fcp_smarty->assign('searchConf', $_POST['searchConf']);
			$fcp_smarty->assign('showOnlyAssignedConf', $_POST['showOnlyAssignedConf']);
            $template = "admin_configurable.tpl";
        }
        elseif($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="delete_configurable") {
            $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
            $result = $fusecp_configurable->deleteConfigurableOption($_POST['id']);
			$fcp_smarty->assign('searchConf', $_POST['searchConf']);
			$fcp_smarty->assign('showOnlyAssignedConf', $_POST['showOnlyAssignedConf']);
            $template = "admin_configurable.tpl";
        }
        elseif($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="edit_addon") {
            $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
            $result = $fusecp_addon->setAddonAutomation($_POST);
			$fcp_smarty->assign('searchAddon', $_POST['searchAddon']);
			$fcp_smarty->assign('showOnlyAssignedAddon', $_POST['showOnlyAssignedAddon']);
            $template = "admin_addon.tpl";
        }
        elseif($_POST['ajax']==1 && $_POST['module']=="fusecp_module" && $_POST['action']=="delete_addon") {
            $aInt->adminTemplate = ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS."ajax";
            $result = $fusecp_addon->deleteAddonAutomation($_POST['id']);
			$fcp_smarty->assign('searchAddon', $_POST['searchAddon']);
			$fcp_smarty->assign('showOnlyAssignedAddon', $_POST['showOnlyAssignedAddon']);
            $template = "admin_addon.tpl";
        }
        else $template = "admin.tpl";
        if($fusecp_settings->settings['ConfigurableOptionsActive'] == 1){
            $res = $fusecp_configurable->getConfigurableOptions();
			if ($res['status'] == 'error') $result = $res;
            $fcp_smarty->assign('configurableoptions',$fusecp_configurable->configurableoptions);
        }
        if($fusecp_settings->settings['AddonsActive'] == 1){
            $res = $fusecp_addon->getAddonAutomation();
			if ($res['status'] == 'error') $result = $res;
            $fcp_smarty->assign('addonautomation',$fusecp_addon->addonautomation);
        }

    }
    $fcp_smarty->assign('result',$result);
    $fcp_smarty->display(ROOTDIR.DS."modules".DS."addons".DS."fusecp_module".DS."templates".DS.$template);
}
