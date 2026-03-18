<?php if (!defined('WHMCS')) exit('ACCESS DENIED');
// Copyright (C) 2026 FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license

/**
 * FuseCP Input Validator
 *
 * Centralised input sanitisation and validation for all data received from
 * WHMCS hooks, module calls and admin-panel form submissions before it is
 * passed to the FuseCP Enterprise Server or stored in the database.
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

class FuseCP_InputValidator
{
    /**
     * Sanitise and validate a WHMCS user ID.
     *
     * @param mixed $value
     * @return int
     * @throws InvalidArgumentException
     */
    public static function userId($value): int
    {
        $id = filter_var($value, FILTER_VALIDATE_INT, ['options' => ['min_range' => 1]]);
        if ($id === false) {
            throw new InvalidArgumentException("Invalid user ID: " . self::truncate($value));
        }
        return $id;
    }

    /**
     * Sanitise and validate a generic positive integer ID (service, addon, package …).
     *
     * @param mixed  $value
     * @param string $fieldName Used in exception message for diagnostics.
     * @return int
     * @throws InvalidArgumentException
     */
    public static function positiveInt($value, string $fieldName = 'ID'): int
    {
        $id = filter_var($value, FILTER_VALIDATE_INT, ['options' => ['min_range' => 1]]);
        if ($id === false) {
            throw new InvalidArgumentException("Invalid {$fieldName}: " . self::truncate($value));
        }
        return $id;
    }

    /**
     * Sanitise a plain-text string (strip tags, trim, enforce max length).
     *
     * @param mixed  $value
     * @param int    $maxLength
     * @param string $fieldName
     * @return string
     * @throws InvalidArgumentException
     */
    public static function string($value, int $maxLength = 255, string $fieldName = 'field'): string
    {
        if (!is_scalar($value)) {
            throw new InvalidArgumentException("Invalid {$fieldName}: expected a string value.");
        }
        $clean = trim(strip_tags((string)$value));
        if (mb_strlen($clean) > $maxLength) {
            throw new InvalidArgumentException("{$fieldName} exceeds maximum length of {$maxLength} characters.");
        }
        return $clean;
    }

    /**
     * Sanitise a hostname or IP address.
     *
     * Allows valid hostnames (e.g. enterprise.example.com) and IPv4/IPv6 addresses.
     *
     * @param mixed $value
     * @return string
     * @throws InvalidArgumentException
     */
    public static function host($value): string
    {
        $clean = trim((string)$value);
        if (empty($clean)) {
            throw new InvalidArgumentException("Host/IP address cannot be empty.");
        }
        // IP address
        if (filter_var($clean, FILTER_VALIDATE_IP)) {
            return $clean;
        }
        // Hostname: letters, digits, hyphens, dots; max 253 chars
        if (preg_match('/^[a-zA-Z0-9]([a-zA-Z0-9\-\.]{0,251}[a-zA-Z0-9])?$/', $clean)) {
            return $clean;
        }
        throw new InvalidArgumentException("Invalid host or IP address: " . self::truncate($clean));
    }

    /**
     * Validate a TCP port number (1–65535).
     *
     * @param mixed $value
     * @return int
     * @throws InvalidArgumentException
     */
    public static function port($value): int
    {
        $port = filter_var($value, FILTER_VALIDATE_INT, ['options' => ['min_range' => 1, 'max_range' => 65535]]);
        if ($port === false) {
            throw new InvalidArgumentException("Invalid port number: " . self::truncate($value));
        }
        return $port;
    }

    /**
     * Validate an email address.
     *
     * @param mixed $value
     * @return string
     * @throws InvalidArgumentException
     */
    public static function email($value): string
    {
        $clean = trim((string)$value);
        if (filter_var($clean, FILTER_VALIDATE_EMAIL) === false) {
            throw new InvalidArgumentException("Invalid email address: " . self::truncate($clean));
        }
        return $clean;
    }

    /**
     * Validate a boolean-like value (e.g. 'on', '1', 'true', 1, true).
     *
     * @param mixed $value
     * @return bool
     */
    public static function boolean($value): bool
    {
        if (is_bool($value)) {
            return $value;
        }
        $lower = strtolower(trim((string)$value));
        return in_array($lower, ['1', 'on', 'true', 'yes'], true);
    }

    /**
     * Truncate a value to a safe display length for use in error messages.
     */
    private static function truncate($value, int $len = 40): string
    {
        $s = is_scalar($value) ? (string)$value : gettype($value);
        return mb_strlen($s) > $len ? mb_substr($s, 0, $len) . '…' : $s;
    }
}
