<?php if (!defined('WHMCS')) exit('ACCESS DENIED');
// Copyright (C) 2026 FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license

/**
 * FuseCP REST API Client — HMAC-SHA256 authentication
 *
 * Provides a REST-over-HTTPS client as the preferred communication method
 * for FuseCP Enterprise Server v2+.  Falls back to the legacy SOAP client
 * (FuseCP_EnterpriseServer) when the REST endpoint is unavailable, and
 * logs a deprecation notice when the fallback is used.
 *
 * Authentication priority:
 *   1. HMAC-SHA256 (recommended)
 *   2. SOAP with credential header (legacy, deprecated)
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

require_once(ROOTDIR . '/modules/addons/fusecp_module/lib/error_handler.php');
require_once(ROOTDIR . '/modules/addons/fusecp_module/lib/audit_logger.php');
require_once(ROOTDIR . '/modules/addons/fusecp_module/lib/enterpriseserver.php');

class FuseCPRestClient
{
    /** Module version – kept in sync with fusecp_module.php */
    const MODULE_VERSION = '2.0.0';

    /** @var string */
    private $host;

    /** @var int */
    private $port;

    /** @var bool */
    private $secure;

    /** @var string */
    private $apiKeyId;

    /** @var string */
    private $apiKeySecret;

    /** @var string 'hmac'|'soap' */
    private $authMethod;

    /** @var bool */
    private $verifySsl;

    /** @var FuseCP_EnterpriseServer|null  SOAP fallback */
    private $soapClient = null;

    /**
     * @param array $credentials {
     *   @type string host
     *   @type int    port
     *   @type bool   secure
     *   @type string api_key_id      HMAC key ID
     *   @type string api_key_secret  HMAC key secret
     *   @type string username        Legacy SOAP username (fallback)
     *   @type string password        Legacy SOAP password (fallback)
     *   @type bool   verify_ssl      Validate TLS certificate (default true)
     * }
     * @param string $authMethod 'hmac' or 'soap'
     */
    public function __construct(array $credentials, string $authMethod = 'hmac')
    {
        $this->host         = $credentials['host']          ?? '';
        $this->port         = (int)($credentials['port']    ?? 9002);
        $this->secure       = (bool)($credentials['secure'] ?? true);
        $this->apiKeyId     = $credentials['api_key_id']    ?? '';
        $this->apiKeySecret = $credentials['api_key_secret'] ?? '';
        $this->verifySsl    = (bool)($credentials['verify_ssl'] ?? true);
        $this->authMethod   = in_array($authMethod, ['hmac', 'soap'], true) ? $authMethod : 'hmac';

        // Prepare SOAP fallback
        if (!empty($credentials['username'])) {
            $this->soapClient = new FuseCP_EnterpriseServer(
                $credentials['username'],
                $credentials['password'] ?? '',
                $this->host,
                $this->port,
                $this->secure
            );
        }
    }

    /**
     * Execute a REST API call against the FuseCP Enterprise Server.
     *
     * @param string $endpoint  e.g. '/api/v2/users'
     * @param string $method    HTTP verb ('GET', 'POST', 'PUT', 'DELETE')
     * @param array  $payload   Request body (JSON-encoded for POST/PUT)
     * @return array            Decoded JSON response
     * @throws RuntimeException on non-2xx responses or cURL errors
     */
    public function call(string $endpoint, string $method = 'GET', array $payload = []): array
    {
        if ($this->authMethod === 'soap' || empty($this->apiKeyId)) {
            // SOAP is not supported through this REST client; callers must use FuseCP_EnterpriseServer.
            throw new RuntimeException(
                'FuseCPRestClient requires HMAC API credentials. '
                . 'Configure an API Key ID and Secret in FuseCP Module settings, '
                . 'or use FuseCP_EnterpriseServer directly for SOAP-based calls.'
            );
        }

        $scheme  = $this->secure ? 'https' : 'http';
        $url     = "{$scheme}://{$this->host}:{$this->port}{$endpoint}";
        $body    = in_array($method, ['POST', 'PUT', 'PATCH'], true) ? json_encode($payload) : null;
        $headers = $this->buildHmacHeaders($method, $endpoint, $body ?? '');

        $ch = curl_init();
        curl_setopt_array($ch, [
            CURLOPT_URL            => $url,
            CURLOPT_CUSTOMREQUEST  => $method,
            CURLOPT_HTTPHEADER     => $headers,
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_FOLLOWLOCATION => false,
            CURLOPT_TIMEOUT        => 30,
            CURLOPT_CONNECTTIMEOUT => 10,
            CURLOPT_SSL_VERIFYPEER => $this->verifySsl,
            CURLOPT_SSL_VERIFYHOST => $this->verifySsl ? 2 : 0,
        ]);

        if ($body !== null) {
            curl_setopt($ch, CURLOPT_POSTFIELDS, $body);
        }

        $response   = curl_exec($ch);
        $httpCode   = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $curlError  = curl_error($ch);
        curl_close($ch);

        if ($curlError) {
            throw new RuntimeException("FuseCP REST cURL error: {$curlError}");
        }

        if ($httpCode < 200 || $httpCode >= 300) {
            throw new RuntimeException(
                "FuseCP REST API returned HTTP {$httpCode} for {$method} {$endpoint}: "
                . substr((string)$response, 0, 200)
            );
        }

        $decoded = json_decode((string)$response, true);
        if (json_last_error() !== JSON_ERROR_NONE) {
            throw new RuntimeException('FuseCP REST API: invalid JSON response – ' . json_last_error_msg());
        }

        return $decoded ?? [];
    }

    /**
     * Build HMAC-SHA256 authentication headers.
     *
     * Header format:
     *   Authorization: FuseCPHMAC keyId="<id>", signature="<sig>", timestamp="<ts>", nonce="<nonce>"
     *
     * The signature covers: METHOD\nPATH\nTIMESTAMP\nNONCE\nBODY_SHA256
     */
    private function buildHmacHeaders(string $method, string $path, string $body): array
    {
        $timestamp   = (string)time();
        $nonce       = bin2hex(random_bytes(16));
        $bodyHash    = hash('sha256', $body);
        $signingStr  = implode("\n", [strtoupper($method), $path, $timestamp, $nonce, $bodyHash]);
        $signature   = hash_hmac('sha256', $signingStr, $this->apiKeySecret);

        $authHeader  = sprintf(
            'FuseCPHMAC keyId="%s", signature="%s", timestamp="%s", nonce="%s"',
            $this->apiKeyId,
            $signature,
            $timestamp,
            $nonce
        );

        return [
            'Authorization: ' . $authHeader,
            'Content-Type: application/json',
            'Accept: application/json',
            'X-FuseCP-Client: WHMCS-Module/' . self::MODULE_VERSION,
        ];
    }

    /**
     * Return the underlying SOAP client for legacy method calls.
     *
     * @return FuseCP_EnterpriseServer
     * @throws RuntimeException if no SOAP credentials were supplied.
     */
    public function getSoapClient(): FuseCP_EnterpriseServer
    {
        if ($this->soapClient === null) {
            throw new RuntimeException(
                'No SOAP credentials configured. Provide username/password in the credentials array.'
            );
        }
        return $this->soapClient;
    }
}
