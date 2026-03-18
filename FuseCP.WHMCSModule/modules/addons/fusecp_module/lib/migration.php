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
 * FuseCP migration controller
 * 
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 * @final
 */

require_once (ROOTDIR. '/modules/addons/fusecp_module/lib/var_definition.php');

use Illuminate\Database\Capsule\Manager as Capsule;

function checkMigration(){
    $needTableMigration = needTableMigration();
    $needDbValuesMigration = needDbValuesMigration();
    $needModulesDeactivation = needModulesDeactivation();
    $needFilesDeletion = needFilesDeletion();
    
    if($needTableMigration['status']== 'error') return $needTableMigration;
    if($needDbValuesMigration['status']== 'error') return $needDbValuesMigration;
    
    try{
        if(!$needTableMigration && !$needDbValuesMigration && !$needModulesDeactivation && !$needFilesDeletion){
            Capsule::table(SOLIDCP_SETTINGS_TABLE)
                    ->where('setting', 'NeedMigration')
                    ->update(['value' => 0, 'updated_at' => date('Y-m-d H:i:s')]);
        }
        else{
            Capsule::table(SOLIDCP_SETTINGS_TABLE)
                    ->where('setting', 'NeedMigration')
                    ->update(['value' => 1, 'updated_at' => date('Y-m-d H:i:s')]);
        }
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't update the database migration status: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    return true;
}

function startMigration($command="",$value1="",$value2="",$option=""){
    $command = strip_tags($command);
    $value1 = strip_tags($value1);
    $value2 = strip_tags($value2);
    $option = strip_tags($option);
    switch($command){
        case 'migrateTable':
            $needTableMigration = needTableMigration();
            $result = "";
            foreach($needTableMigration as $value){
                if($value['old_tbl'] == $value1 && $value['new_tbl'] == $value2){
                    if($option == "copy") $result = migrateTable($value1, $value2);
                    elseif($option == "delete") $result = deleteTable($value1);
                    else return array('status' => 'error', 'description' => "Wrong option for table migration. Need to be 'copy' or 'delete'!");
                }
            }
            if($result['status']=='error') return $result;
            elseif($result['status']=='success'){
                $needTableMigration = needTableMigration();
                foreach($needTableMigration as $value){
                    if($value['old_tbl'] == $value1 && $value['new_tbl'] == $value2){
                        return array('status' => 'error', 'description' => "Operation failed due to an unspecified error!");
                    }
                }
                return $result;
            }
            else return array('status' => 'error', 'description' => "Couldn't find the specified tables for copy!");
            break;
        case 'migrateDbValues':
            $needDbValuesMigration = needDbValuesMigration();
            $result = "";
            foreach($needDbValuesMigration as $value){
                if($value['table'] == $value1 && $value['depr_value'] == $value2){
                    if($option == "migrate") $result = migrateDbValues($value1, $value2);
                    else return array('status' => 'error', 'description' => "Wrong option for values migration. Need to be 'migrate'!");
                }
            }
            if($result['status']=='error') return $result;
            elseif($result['status']=='success'){
                $needDbValuesMigration = needDbValuesMigration();
                foreach($needDbValuesMigration as $value){
                    if($value['table'] == $value1 && $value['depr_value'] == $value2){
                        return array('status' => 'error', 'description' => "Operation failed due to an unspecified error!");
                    }
                }
                return $result;
            }
            else return array('status' => 'error', 'description' => "Couldn't find the specified values for migration!");
            break;
        case 'deactivateModules':
            $needModulesDeactivation = needModulesDeactivation();
            $result = true;
            foreach($needModulesDeactivation as $value){
                if($value == $value1){
                    $result = false;
                }
            }
            if($result) return array('status' => 'success', 'description' => "Module '$value1' is deactivated!");
            else return array('status' => 'error', 'description' => "Module '$value1' is still active!");
            break;
// not implemented yet!
//        case 'deleteFiles':
//            break;
        default:
            return array('status' => 'error', 'description' => "Couldn't find the specified command for migration!");
    }
}

function migrateDbValues($table = "", $depr_value = ""){
    try{
        if($table == "tblproducts" && $depr_value == "configoption6"){
            $old_tbl_content = Capsule::table($table)
                    ->select('id', 'configoption7', 'configoption8', 'configoption9', 'configoption10', 'configoption11', 'configoption12', 'configoption13', 'configoption14', 'configoption15', 'configoption16', 'configoption17', 'configoption18', 'configoption19', 'configoption20', 'configoption21', 'configoption22', 'configoption23', 'configoption24')
                    ->where('servertype','FuseCP')
                    ->whereRaw('configoption6 REGEXP "^[0-9]{4,5}$"')
                    ->get();
            foreach($old_tbl_content as $value){
                $sqlData = array();
                for($i=7; $i<=24; $i++){
                    $keyname = 'configoption'.($i-1);
                    $valuename = 'configoption'.$i;
                    $sqlData[$keyname] = $value->$valuename;
                }
                Capsule::table($table)
                    ->where('id', $value->id)
                    ->update($sqlData);
            }
        }
        // FURTHER VALUE MIGRATION CODE CAN BE ADDED HERE
        
        // END OF CODE FOR VALUE MIGRATION
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't migrate the value '".$depr_value."' in table '".$table."': (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    return array('status' => 'success', 'description' => "Values '".$depr_value."' in table '".$table."' successfully migrated");
}

function migrateTable($old_tbl = "", $new_tbl = ""){
    try{
            $old_tbl_content = Capsule::table($old_tbl)
                    ->select('*')
                    ->get();
            $sqlData = array();
            $i = 0;
            foreach($old_tbl_content as $value){
                foreach ($value as $key => $array_value){
                    // ADD FURTHER CODE FOR TRANSLATION OF DATA TYPES OR MODIFYNG OF VALUES HERE
                    if($array_value == chr(0x01)) $array_value = 1;
                    if($key == "is_ipaddress" && $array_value == chr(0x00)) $array_value = 0;
                    
                    // END OF CODE FOR TRANSLATION OR MODIFYING OF VALUES
                    $sqlData[$i][$key] = $array_value;
                }
                
                // ADD FURTHER CODE FOR FILLING NEW COLUMNS WHILE COPYING THE TABLES HERE
                if(!$sqlData[$i]['created_at']) $sqlData[$i]['created_at'] = date('Y-m-d H:i:s');

                // END OF CODE FOR FILLING NEW COLUMNS
                $i++;
            }
            Capsule::table($new_tbl)
                    ->insert($sqlData);
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't copy '".$old_tbl."' to '".$new_tbl."': (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    return array('status' => 'success', 'description' => "Table successfully migrated");
}

function deleteTable($old_tbl = ""){
    try{
            Capsule::schema()->dropIfExists($old_tbl);
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't copy '".$old_tbl."' to '".$new_tbl."': (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    return array('status' => 'success', 'description' => "Table successfully deleted");
}

function getHTMLids($command="",$value1="",$value2=""){
    $command = strip_tags($command);
    $value1 = strip_tags($value1);
    $value2 = strip_tags($value2);
    $html_ids['box'] = "box_".$command."_".$value1."_".$value2;
    $html_ids['button'] = "button_".$command."_".$value1."_".$value2;
    $html_ids['icon'] = "icon_".$command."_".$value1."_".$value2;
    return $html_ids;
}

function migrationSteps(){
    $migrationSteps = array();
    $needTableMigration = needTableMigration();
    $needDbValuesMigration = needDbValuesMigration();
    $needModulesDeactivation = needModulesDeactivation();
    $needFilesDeletion = needFilesDeletion();
    
    if($needTableMigration['status']== 'error') return $needTableMigration;
    if($needDbValuesMigration['status']== 'error') return $needDbValuesMigration;

    if(count($needTableMigration) > 0){
        foreach ($needTableMigration as $value) {
            $migrationSteps[] = ['command' => 'migrateTable', 'value1' => $value['old_tbl'], 'value2' => $value['new_tbl']];
        }
    }
    if(count($needDbValuesMigration) > 0){
        foreach ($needDbValuesMigration as $value) {
            $migrationSteps[] = ['command' => 'migrateDbValues', 'value1' => $value['table'], 'value2' => $value['depr_value']];
        }
    }
    if(count($needModulesDeactivation) > 0){
        foreach ($needModulesDeactivation as $value) {
            $migrationSteps[] = ['command' => 'deactivateModules', 'value1' => $value, 'value2' => ''];
        }
    }
    if(count($needFilesDeletion) > 0){
        foreach ($needFilesDeletion as $value) {
            $migrationSteps[] = ['command' => 'deleteFiles', 'value1' => $value, 'value2' => ''];
        }
    }
    if(count($migrationSteps) > 0) return $migrationSteps;
    else return false;
}

function needTableMigration(){
    $migrate_old_tables = array();
    try{
        // Before version 1.0.1 probably Websitepanel? Will be changed in upcoming version!
        if(Capsule::schema()->hasTable('tblfcpaddons')){
            $old_tbl_addons = Capsule::table('tblfcpaddons')
                    ->select('*')
                    ->get();
            $new_tbl_addons = Capsule::table(SOLIDCP_ADDONS_TABLE)
                    ->select('*')
                    ->get();
            if(count($old_tbl_addons) > 0 && count($new_tbl_addons) <= 0) $migrate_old_tables[] = ['old_tbl' => 'tblfcpaddons', 'new_tbl' => SOLIDCP_ADDONS_TABLE];
        }
        
        // Before version 1.1.0
        if(Capsule::schema()->hasTable('mod_fcpaddons')){
            $old_tbl_addons = Capsule::table('mod_fcpaddons')
                    ->select('*')
                    ->get();
            $new_tbl_addons = Capsule::table(SOLIDCP_ADDONS_TABLE)
                    ->select('*')
                    ->get();
            if(count($old_tbl_addons) > 0 && count($new_tbl_addons) <= 0) $migrate_old_tables[] = ['old_tbl' => 'mod_fcpaddons', 'new_tbl' => SOLIDCP_ADDONS_TABLE];
        }
        
        // Before version 1.1.0
        if(Capsule::schema()->hasTable('mod_fcpconfigurable')){
            $old_tbl_addons = Capsule::table('mod_fcpconfigurable')
                    ->select('*')
                    ->get();
            $new_tbl_addons = Capsule::table(SOLIDCP_CONFIGURABLE_OPTIONS_TABLE)
                    ->select('*')
                    ->get();
            if(count($old_tbl_addons) > 0 && count($new_tbl_addons) <= 0) $migrate_old_tables[] = ['old_tbl' => 'mod_fcpconfigurable', 'new_tbl' => SOLIDCP_CONFIGURABLE_OPTIONS_TABLE];
        }
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't get old or new tables for migration status: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    if(count($migrate_old_tables)>0) return $migrate_old_tables;
    else return false;
}

function needDbValuesMigration(){
    $migrate_db_values = array();

    try{
        // Check if field "port" (9002) is still in the products database / Field was depracated in 1.1.0
        if(Capsule::schema()->hasTable('tblproducts')){
            $old_tbl_values = Capsule::table('tblproducts')
                    ->select('id', 'name')
                    ->where('servertype','FuseCP')
                    ->whereRaw('configoption6 REGEXP "^[0-9]{4,5}$"')
                    ->get();
            if(count($old_tbl_values) > 0) $migrate_db_values[] = ['table' => 'tblproducts', 'depr_value' => 'configoption6'];
        }
    }
    catch (Exception $e){
        return array('status' => 'error', 'description' => "Couldn't get database values to check, if they need a migration: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
    }
    if(count($migrate_db_values)>0) return $migrate_db_values;
    else return false;
}

function needModulesDeactivation(){
    $deactive_old_modules = array();
    foreach($GLOBALS['activeaddonmodules'] as $value){
        if($value == 'fusecp_addons') $deactive_old_modules[]='fusecp_addons';
        if($value == 'fusecp_configurable') $deactive_old_modules[]='fusecp_configurable';
        if($value == 'fusecp_sync') $deactive_old_modules[]='fusecp_sync';
    }
    if(count($deactive_old_modules)>0) return $deactive_old_modules;
    else return false;
}

function needFilesDeletion(){
    $migrate_delete_files = array();
    // Before version 1.1.0
    $migrate_old_files[] = '/modules/addons/fusecp_addons/hooks.php';
    $migrate_old_files[] = '/modules/addons/fusecp_addons/fusecp_addons.php';
    $migrate_old_files[] = '/modules/addons/fusecp_configurable/fusecp_configurable.php';
    $migrate_old_files[] = '/modules/addons/fusecp_sync/hooks.php';
    $migrate_old_files[] = '/modules/addons/fusecp_sync/fusecp_sync.php';
    $migrate_old_files[] = '/modules/servers/FuseCP/enterpriseserver.php';
    $migrate_old_files[] = '/modules/servers/FuseCP/functions.php';
    $migrate_old_files[] = '/modules/servers/FuseCP/clientarea.tpl';

    foreach($migrate_old_files as $value){
        if(is_file(ROOTDIR.$value)) $migrate_delete_files[]= $value;
    }
    if(count($migrate_delete_files)>0) return $migrate_delete_files;
    else return false;
}
