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
 * FuseCP module settings model
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

Class fusecp_settings{
    
    public $admins = NULL;
    public $settings = NULL;
    
    public function getSettings(){
        try{
            $this->admins = Capsule::table('tbladmins')
                    ->select('username','firstname','lastname','email','signature','language')
                    ->where('roleid','=',1)
                    ->where('disabled','=',0)
                    ->get();
        }
        catch (Exception $e){
            return array('status' => 'error', 'description' => "Couldn't fetch the WHMCS admin list: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
        }
        
        try{
            $db_settings = Capsule::table(SOLIDCP_SETTINGS_TABLE)
                    ->select('*')
                    ->get();
            foreach ($db_settings as $value){
                $this->settings[$value->setting] = $value->value;
            }
        }
        catch (Exception $e){
            return array('status' => 'error', 'description' => "Couldn't read the config table from database: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
        }
    }
    
    public function setSettings($new_settings){
        unset($new_settings['ajax']);
        unset($new_settings['token']);
        unset($new_settings['tab']);
        unset($new_settings['action']);
        unset($new_settings['module']);
        
        $old_settings = $this->settings;
        if($new_settings['WhmcsAdmin'] == "") return array('status' => 'error', 'description' => "WHMCS Admin can't be empty. Please select an admin."); 
        try{
            foreach ($old_settings as $key => $value){
                if($new_settings[$key]){
                    $new_settings[$key] = strip_tags($new_settings[$key]);
                    if($new_settings[$key] == "on") $new_settings[$key] = 1;
                    if($new_settings[$key] != $value){
                        Capsule::table(SOLIDCP_SETTINGS_TABLE)
                            ->where('setting', $key)
                            ->update(['value' => $new_settings[$key], 'updated_at' => date('Y-m-d H:i:s')]);
                    }
                }
                elseif($value == 1){
                        Capsule::table(SOLIDCP_SETTINGS_TABLE)
                            ->where('setting', $key)
                            ->update(['value' => 0, 'updated_at' => date('Y-m-d H:i:s')]);
                }
            }
        }
        catch (Exception $e){
            return array('status' => 'error', 'description' => "Couldn't save the settings to database: (Code: {$e->getCode()}, Message: {$e->getMessage()}");
        }
        return array('status' => 'success', 'description' => "Settings successfully saved.");
    }
}
