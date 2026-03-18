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
 * FuseCP WHMCS Server Module Shared Funcitons
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

/**
 * Calculate the bandwidth calculation starting date based on when the customer signed up
 * 
 * @access public
 * @param string $startDate Customer registration date (starting date)
 * @return string
 */
function FuseCP_CreateBandwidthDate($startDate)
{
	$dateExploded = explode('-', $startDate);
	$currentYear = date('Y');
	$currentMonth = date('m');
	$newDate = "{$currentYear}-{$currentMonth}-{$dateExploded[2]}";
	
	$dateDiff = time() - strtotime('+1 hour', strtotime($newDate));
	$fullDays = floor($dateDiff / (60 * 60 * 24));
	if ($fullDays < 0)
	{
		return date('Y-m-d', strtotime('-1 month', strtotime($newDate)));
	}
	else
	{
		return $newDate;
	}
}

/**
 * Calculates the total usage of the provided FuseCP usage tables
 * Each FuseCP provider / service calculates its own disk and bandwidth usage, total all provided tables and return
 * 
 * @access public
 * @param array $usageTables Usage tables (each service provides its own usage breakdown)
 * @param int $usageType FuseCP usage type (FuseCP_EnterpriseServer::USAGE_*)
 * @return int
 */
function FuseCP_CalculateUsage($usageTables, $usageType)
{
	$totalUsage = 0;

	if (is_array($usageTables['NewDataSet']['Table'][0])) {
        $usageTable = $usageTables['NewDataSet']['Table'];
    }
    else {
        $usageTable = $usageTables['NewDataSet'];
    }

	foreach ($usageTable as $table)
	{
		switch ($usageType)
		{
		    case FuseCP_EnterpriseServer::USAGE_BANDWIDTH:
				$totalUsage += $table['BytesTotal'];
		    break;
		    
		    case FuseCP_EnterpriseServer::USAGE_DISKSPACE:
				$totalUsage += $table['DiskspaceBytes'];
		    break;
		}
	}

	if ($totalUsage > 0) { $totalUsage = round($totalUsage / 1024 / 1024); }

	return $totalUsage;
}

/**
 * Loads the FuseCP language file
 * 
 * @access public
 */
function FuseCP_LoadClientLanguage()
{
    global $CONFIG, $_LANG, $smarty;
    
    // Attempt to load the client's language
    $selectedLanguage = !empty($_SESSION['Language']) ? $_SESSION['Language'] : $CONFIG['Language'];
    
    // For the admin area
    if (defined('ADMINAREA'))
    {
        $admin = fusecp_database::getAdminLanguage((int)$_SESSION['adminid']);
        $selectedLanguage = !empty($admin->language) ? $admin->language : 'english';
    }
    
    // Load the language file
    $languageFile = ROOTDIR . "/modules/servers/FuseCP/lang/{$selectedLanguage}.php";
    if (file_exists($languageFile))
    {
        require_once($languageFile);
    }
    else
    {
        // Load the default (English) language file
        require_once( ROOTDIR . '/modules/servers/FuseCP/lang/english.php');
    }
    
    // Process the module language entries
    if (is_array($_MOD_LANG))
    {
        foreach ($_MOD_LANG as $key => $value)
        {
            if (empty($_LANG[$key]))
            {
                $_LANG[$key] = $value;
            }
        }
    }
    
    // Add to the template
    if (isset($smarty))
    {
        $smarty->assign('LANG', $_LANG);
    }
    
    return $_MOD_LANG;
}
