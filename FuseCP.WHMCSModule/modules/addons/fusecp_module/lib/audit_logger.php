<?php if (!defined('WHMCS')) exit('ACCESS DENIED');
// Copyright (C) 2026 FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license

/**
 * FuseCP Audit Logger
 *
 * Provides centralized audit trail logging for all FuseCP API operations,
 * credential changes, and security-relevant events.
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

require_once(ROOTDIR . '/modules/addons/fusecp_module/lib/var_definition.php');

use Illuminate\Database\Capsule\Manager as Capsule;

class FuseCP_AuditLogger
{
    /**
     * Log a FuseCP API action to the WHMCS activity log and the FuseCP audit table.
     *
     * @param string $action    Short action name, e.g. 'CREATE_ACCOUNT'
     * @param int    $userId    WHMCS client user ID (0 for system actions)
     * @param string $status    'success' or 'error'
     * @param string $detail    Human-readable description / error message
     * @param array  $extra     Optional extra context (api_method, service_id, duration_ms …)
     */
    public static function log(string $action, int $userId, string $status, string $detail, array $extra = []): void
    {
        // Always write to WHMCS activity log
        $message = sprintf(
            'FuseCP [%s] %s – %s',
            strtoupper($action),
            strtoupper($status),
            $detail
        );
        if (!empty($extra)) {
            $contextParts = [];
            foreach ($extra as $k => $v) {
                $contextParts[] = "{$k}=" . (is_scalar($v) ? $v : json_encode($v));
            }
            $message .= ' (' . implode(', ', $contextParts) . ')';
        }
        logactivity($message, $userId);

        // Attempt to write to the audit table if it exists
        try {
            if (Capsule::schema()->hasTable(FUSECP_AUDIT_LOG_TABLE)) {
                Capsule::table(FUSECP_AUDIT_LOG_TABLE)->insert([
                    'action'       => substr($action, 0, 100),
                    'userid'       => $userId,
                    'status'       => substr($status, 0, 20),
                    'detail'       => substr($detail, 0, 500),
                    'api_method'   => substr((string)($extra['api_method'] ?? ''), 0, 100),
                    'service_id'   => (int)($extra['service_id'] ?? 0),
                    'duration_ms'  => (int)($extra['duration_ms'] ?? 0),
                    'ip_address'   => substr((string)($extra['ip_address'] ?? self::getClientIp()), 0, 45),
                    'created_at'   => date('Y-m-d H:i:s'),
                ]);
            }
        } catch (Exception $e) {
            // Non-fatal: audit table write failure should not break provisioning
            logactivity('FuseCP AuditLogger: could not write to audit table – ' . $e->getMessage(), 0);
        }
    }

    /**
     * Convenience wrapper for successful operations.
     */
    public static function success(string $action, int $userId, string $detail, array $extra = []): void
    {
        self::log($action, $userId, 'success', $detail, $extra);
    }

    /**
     * Convenience wrapper for failed operations.
     */
    public static function failure(string $action, int $userId, string $detail, array $extra = []): void
    {
        self::log($action, $userId, 'error', $detail, $extra);
    }

    /**
     * Returns the current client IP address (best-effort).
     */
    private static function getClientIp(): string
    {
        foreach (['HTTP_X_FORWARDED_FOR', 'HTTP_CLIENT_IP', 'REMOTE_ADDR'] as $key) {
            if (!empty($_SERVER[$key])) {
                $ip = trim(explode(',', $_SERVER[$key])[0]);
                if (filter_var($ip, FILTER_VALIDATE_IP)) {
                    return $ip;
                }
            }
        }
        return '';
    }
}
