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
 * For the FuseCP system only - Only tested against the FuseCP system
 * 
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
{
	/**
     * FuseCP user account statuses / states
     *
     * @access public
     * @var string
     */
    const USERSTATUS_ACTIVE = 'Active';
    const USERSTATUS_SUSPENDED = 'Suspended';
    const USERSTATUS_CANCELLED = 'Cancelled';
    const USERSTATUS_PENDING = 'Pending';
    
    /**
     * FuseCP usage calculation types
     *
     * @access public
     * @var int
     */
    const USAGE_DISKSPACE = 0;
    const USAGE_BANDWIDTH = 1;
    
	/**
	 * Enterprise Server username
	 *
	 * @access private
	 * @var string
	 */
	private $_username;
	
	/**
	 * Enterprise Server password
	 *
	 * @access private
	 * @var string
	 */
	private $_password;
	
	/**
	 * Enterprise Server URL / address (without the port)
	 *
	 * @access private
	 * @var string
	 */
	private $_host;
	
	/**
	 * Enterprise Server TCP port
	 *
	 * @access private
	 * @var int
	 */
	private $_port;
	
	/**
	 * Use SSL (HTTPS) for Enterprise Server communications
	 *
	 * @access private
	 * @var boolean
	 */
	private $_secured;
	
	/**
	 * SoapClient WSDL caching
	 * 
	 * @access private
	 * @var boolean
	 */
	private $_caching;
	
	/**
	 * SoapClient HTTP compression
	 * 
	 * @access private
	 * @var boolean
	 */
	private $_compression;

	/**
	 * Verify SSL peer certificate when using HTTPS.
	 * Disable only for development/testing environments.
	 *
	 * @access private
	 * @var boolean
	 */
	private $_verifySsl;

	/**
	 * Class constructor
	 * 
	 * @access public
	 * @param string $username
	 * @param string $password
	 * @param string $host
	 * @param int $port
	 * @param boolean $secured
	 * @param boolean $caching
	 * @param boolean $compression
	 * @param boolean $verifySsl  Verify SSL peer certificate (default TRUE when $secured is TRUE)
	 */
	function __construct($username, $password, $host, $port = 9002, $secured = FALSE, $caching = FALSE, $compression = TRUE, $verifySsl = TRUE)
	{
		$this->_username = $username;
		$this->_password = $password;
		$this->_host = $host;
		$this->_port = $port;
		$this->_secured = $secured;
		$this->_caching = $caching;
		$this->_compression = $compression;
		$this->_verifySsl = $verifySsl;
	}
	
	/**
	 * Executes the "CreateUserWizard" method
	 * 
	 * @param array $params CreateUserWizard method parameters
	 * @throws Exception
	 * @return int
	 */
	public function createUserWizard($params)
	{
		try
		{
		    return $this->execute('esPackages', 'CreateUserWizard', $params)->CreateUserWizardResult;
		}
		catch (Exception $e)
		{
			throw new Exception("ChangeUserStatus Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}

        
	/**
	 * Executes the "UpdateUser" method
	 * 
	 * @access private
	 * @param array $params
	 * @throws Exception
	 * @return int
	 */
	public function updateUserDetails($params)
	{
		try
		{
		    return $this->execute('esUsers', 'UpdateUser', array('user' => $params))->UpdateUserResult;
		}
		catch (Exception $e)
		{
			throw new Exception("UpdateUser Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "DeleteUser" method
	 * 
	 * @access public
	 * @param int $userId User's FuseCP userId
	 * @throws Exception
	 * @return int
	 */
	public function deleteUser($userId)
	{
		try
		{
		    return $this->execute('esUsers', 'DeleteUser', array('userId' => $userId))->DeleteUserResult;
		}
		catch (Exception $e)
		{
			throw new Exception("ChangeUserStatus Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "GetUserByUsername" method
	 * 
	 * @access public
	 * @param string $username FuseCP username
	 * @throws Exception
	 * @return array
	 */
	public function getUserByUsername($username)
	{
		try
		{
		    return $this->convertArray($this->execute('esUsers', 'GetUserByUsername', array('username' => $username))->GetUserByUsernameResult);
		}
		catch (Exception $e)
		{
			throw new Exception("GetUserByUsername Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()})", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "ChangeUserStatus" method
	 * 
	 * @access public
	 * @param int $userId User's FuseCP userId
	 * @param string $status Account status (Active, Suspended, Cancelled, Pending)
	 * @throws Exception
	 * @return int
	 */
	public function changeUserStatus($userId, $status)
	{
		try
		{
		    return $this->execute('esUsers', 'ChangeUserStatus', array('userId' => $userId, 'status' => $status))->ChangeUserStatusResult;
		}
		catch (Exception $e)
		{
			throw new Exception("ChangeUserStatus Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "ChangeUserPassword" method
	 * 
	 * @access public
	 * @param int $userId User's FuseCP userId
	 * @param string $password User's new password
	 * @throws Exception
	 * @return int
	 */
	public function changeUserPassword($userId, $password)
	{
		try
		{
			return $this->execute('esUsers', 'ChangeUserPassword', array('userId' => $userId, 'password' => $password))->ChangeUserPasswordResult;
		}
		catch (Exception $e)
		{
			throw new Exception("ChangeUserPassword Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "GetMyPackages" method
	 * 
	 * @access public
	 * @param int $userId User's FuseCP userId
	 * @throws Exception
	 * @return array
	 */
	public function getUserPackages($userId)
	{
		try
		{
			return $this->convertArray($this->execute('esPackages', 'GetMyPackages', array('userId' => $userId))->GetMyPackagesResult->PackageInfo);
		}
		catch (Exception $e)
		{
			throw new Exception("GetMyPackages Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}

	/**
	 * Executes the "GetPackageAddons" method
	 * 
	 * @access public
	 * @param int $packageId User's FuseCP packageId
	 * @throws Exception
	 * @return array
	 */
	public function getPackageAddons($packageId)
	{
		try
		{
			return $this->convertArray($this->execute('esPackages', 'GetPackageAddons', array('packageId' => $packageId))->GetPackageAddonsResult);
		}
		catch (Exception $e)
		{
			throw new Exception("GetPackageAddons Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}

	/**
	 * Executes the "DeletePackageAddon" method
	 * 
	 * @access public
	 * @param int $packageAddonId User's FuseCP packageAddonId
	 * @throws Exception
	 * @return int
	 */
	public function deletePackageAddon($packageAddonId)
	{
		try
		{
			return $this->convertArray($this->execute('esPackages', 'DeletePackageAddon', array('packageAddonId' => $packageAddonId))->DeletePackageAddonResult);
		}
		catch (Exception $e)
		{
			throw new Exception("DeletePackageAddon Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}

	/**
	 * Executes the "GetUsersPagedRecursive" method
	 * 
	 * @access public
	 * @param unknown $userId Users's FuseCP userId
	 * @param unknown $filterColumn Column value to filter against
	 * @param unknown $filterValue Filter value
	 * @param unknown $statusId Users's account status id
	 * @param unknown $roleId User's account role id
	 * @param unknown $sortColumn Column value to sort against
	 * @param number $startRow Start value
	 * @param string $maximumRows Maximum rows to return
	 * @throws Exception
	 * @return object
	 */
	public function getUsersPagedRecursive($userId, $filterColumn, $filterValue, $statusId, $roleId, $sortColumn, $startRow = 0, $maximumRows = PHP_INT_MAX)
	{
		try
		{
			$result = (array)$this->execute('esUsers', 'GetUsersPagedRecursive', array('userId' => $userId, 'filterColumn' => $filterColumn, 'filterValue' => $filterValue, 'statusId' => $statusId, 'roleId' => $roleId, 'sortColumn' => $sortColumn, 'startRow' => $startRow, 'maximumRows' => $maximumRows))->GetUsersPagedRecursiveResult;
			return $this->convertArray($result['any'], TRUE);
		}
		catch (Exception $e)
		{
			throw new Exception("GetUsersPagedRecursive Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "UpdatePackageLiteral" method
	 * 
	 * @access public
	 * @param int $packageId Package's FuseCP packageId
	 * @param int $statusId Package's status id
	 * @param int $planId Package's FuseCP planid
	 * @param string $purchaseDate Package's purchase date
	 * @param string $packageName Package's name
	 * @param string $packageComments Package's comments
	 * @throws Exception
	 * @return array
	 */
	public function updatePackageLiteral($packageId, $statusId, $planId, $purchaseDate, $packageName, $packageComments)
	{
		try
		{
			return $this->convertArray($this->execute('esPackages', 'UpdatePackageLiteral', array('packageId' => $packageId, 'statusId' => $statusId, 'planId' => $planId, 'purchaseDate' => $purchaseDate, 'packageName' => $packageName, 'packageComments' => $packageComments))->UpdatePackageLiteralResult);
		}
		catch (Exception $e)
		{
			throw new Exception("UpdatePackageLiteral Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "AddPackageAddonById" method
	 * 
	 * @access public
	 * @param unknown $packageId FuseCP package Id
	 * @param unknown $addonPlanId FuseCP addon id
	 * @param number $quantity Number of addons to add :)
	 * @throws Exception
	 * @return array
	 */
	public function addPackageAddonById($packageId, $addonPlanId, $quantity = 1)
	{
		try
		{
			return $this->convertArray($this->execute('esPackages', 'AddPackageAddonById', array('packageId' => $packageId, 'addonPlanId' => $addonPlanId, 'quantity' => $quantity))->AddPackageAddonByIdResult);
		}
		catch (Exception $e)
		{
			throw new Exception("AddPackageAddonById Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "GetPackageBandwidth" method
	 * 
	 * @access public
	 * @param unknown $packageId FuseCP package id
	 * @param unknown $startDate Calculation start date
	 * @param unknown $endDate Calculation end date
	 * @throws Exception
	 * @return object
	 */
	public function getPackageBandwidthUsage($packageId, $startDate, $endDate)
	{
		try
		{
			$result = (array)$this->execute('esPackages', 'GetPackageBandwidth', array('packageId' => $packageId, 'startDate' => $startDate, 'endDate' => $endDate))->GetPackageBandwidthResult;
			return $this->convertArray($result['any'], TRUE);
		}
		catch (Exception $e)
		{
			throw new Exception("GetPackageBandwidth Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "GetPackageDiskspace" method
	 *
	 * @access public
	 * @param unknown $packageId FuseCP package id
	 * @throws Exception
	 * @return object
	 */
	public function getPackageDiskspaceUsage($packageId)
	{
		try
		{
			$result = (array)$this->execute('esPackages', 'GetPackageDiskspace', array('packageId' => $packageId))->GetPackageDiskspaceResult;
			return $this->convertArray($result['any'], TRUE);
		}
		catch (Exception $e)
		{
			throw new Exception("GetPackageDiskspace Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Executes the "AllocatePackageIPAddresses" method
	 * 
	 * @access public
	 * @param int $packageId FuseCP package id
	 * @param string $groupName FuseCP IP address group name
	 * @param string $pool FuseCP IP address pool
	 * @param int $addressesNumber Number of IP addresses to allocate
	 * @param string $allocateRandom Allocate randomly
	 * @throws Exception
	 * @return object
	 */
	public function allocatePackageIPAddresses($packageId, $groupName = 'Web', $pool = 'WebSites', $addressesNumber = 1, $allocateRandom = TRUE)
	{
		try
		{
			return $this->convertArray($this->execute('esServers', 'AllocatePackageIPAddresses', array('packageId' => $packageId, 'groupName' => $groupName, 'pool' => $pool, 'addressesNumber' => $addressesNumber, 'allocateRandom' => $allocateRandom))->AllocatePackageIPAddressesResult);
		}
		catch (Exception $e)
		{
			throw new Exception("AllocatePackageIPAddresses Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()}", $e->getCode(), $e);
		}
	}
	
	/**
	 * Converts the FuseCP error code to a friendly human-readable message
	 * 
	 * @access public
	 * @param int $code FuseCP error code
	 * @return  string
	 */
	public static function getFriendlyError($code)
	{
		$errors = array(-100 => 'Username not available, already in use',
					  	-101 => 'Username not found, invalid username',
					  	-102 => 'User\'s account has child accounts',
					  	-300 => 'Hosting package could not be found',
					  	-301 => 'Hosting package has child hosting spaces',
					  	-501 => 'The sub-domain belongs to an existing hosting space that does not allow sub-domains to be created',
					  	-502 => 'The domain or sub-domain exists in another hosting space / user account',
					  	-511 => 'Preview Domain is enabled, but not configured',
					  	-601 => 'The website already exists on the target hosting space or server',
					  	-700 => 'The email domain already exists on the target hosting space or server',
					  	-1100 => 'User already exists');
		
		// Find the error and return it, else a general error will do!
		if (array_key_exists($code, $errors))
		{
			return $errors[$code];
		}
		else
		{
			return "An unknown error occured (Code: {$code}). Please reference FuseCP BusinessErrorCodes for further information";
		}
	}
	
	public function testEcho($text) {
        return $this->execute('esTest', 'Echo', array('msg' => $text));
    }	

	public function osPlatform() {
		return $this->execute('esTest', 'OSPlatform', null);
	}
	/**
	 * Executes the request on the Enterprise Server and returns the results
	 * 
	 * @param unknown $service
	 * @param unknown $method
	 * @param unknown $params
	 * @throws Exception
	 */
	public function execute($service, $method, $params)
	{
		// Set the Enterprise Server full URL
		$host = (($this->_secured) ? 'https' : 'http') . "://{$this->_host}:{$this->_port}/{$service}?WSDL";

		// Build SSL stream context for certificate verification
		$sslContext = null;
		if ($this->_secured) {
			$sslContext = stream_context_create([
				'ssl' => [
					'verify_peer'       => $this->_verifySsl,
					'verify_peer_name'  => $this->_verifySsl,
					'allow_self_signed' => !$this->_verifySsl,
				],
			]);
		}

		$clientOptions = array(
			'compression' => (($this->_compression) ? (SOAP_COMPRESSION_ACCEPT | SOAP_COMPRESSION_GZIP) : ''),
			'keep_alive'  => false,
			'cache_wsdl'  => ($this->_caching) ? 1 : 0,
		);
		if ($sslContext !== null) {
			$clientOptions['stream_context'] = $sslContext;
		}

		try
		{
			// Create the SoapClient
			$client = new SoapClient($host, $clientOptions);

			$hasUser = !empty($this->_password);
			if ($hasUser) {
				$pwd = $this->_password;
				$pwd = str_replace("&", "&amp;", $pwd);
				$pwd = str_replace("<", "&lt;", $pwd);
				$pwd = str_replace(">", "&gt;", $pwd);
				$auth = "<Credentials xmlns=\"http://fusecp/headers/Credentials\"><Password xmlns=\"http://hostpanelpro/credentials\">$pwd</Password><Username xmlns=\"http://hostpanelpro/credentials\">$this->_username</Username></Credentials>";
				$auth_block = new SoapVar( $auth, XSD_ANYXML, NULL, NULL, NULL, NULL );
				$soapHeader = new SoapHeader('http://fusecp/headers/Credentials', 'Credentials', $auth_block);
				$client -> __setSoapHeaders($soapHeader);
			}
		} catch (SoapFault $ex) {
			// Old version of FuseCP
			$host = (($this->_secured) ? 'https' : 'http') . "://{$this->_host}:{$this->_port}/{$service}.asmx?WSDL";
			$legacyOptions = array_merge($clientOptions, array('login' => $this->_username, 'password' => $this->_password));
			// Create the SoapClient
			$client = new SoapClient($host, $legacyOptions);
		}
		
		try {
			// Execute the request and process the results
			return call_user_func(array($client, $method), $params);
		}
		catch (SoapFault $e)
		{
			throw new Exception("SOAP Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()})");
		}
		catch (Exception $e)
		{
			throw new Exception("General Fault: (Code: {$e->getCode()}, Message: {$e->getMessage()})");
		}
	}
	
	/**
	 * Converts an object or an XML string to an array
	 * 
	 * @access private
	 * @param mixed $value Object or an XML string
	 * @param boolean $loadXml Loads the string into the SimpleXML object
	 * @return array
	 */
	private function convertArray($value, $loadXml = FALSE)
	{
		// This is silly, but it works, and it works very well for what we are doing :)
		return json_decode(json_encode(($loadXml ? simplexml_load_string($value) : $value)), TRUE);
	}
        
}
