<?php if (!defined('WHMCS')) exit('ACCESS DENIED');
// Copyright (C) 2026 FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license

/**
 * FuseCP Error Handler — retry logic with exponential back-off
 *
 * Wraps any callable and retries on transient errors (SoapFault, connection
 * errors) using configurable exponential back-off with optional jitter.
 *
 * @author FuseCP
 * @link https://fusecp.com/
 * @access public
 * @name FuseCP
 * @version 2.0.0
 * @package WHMCS
 */

class FuseCP_ErrorHandler
{
    /**
     * Execute a callable with exponential back-off retry.
     *
     * @param callable $callable         Function to execute; should throw on failure.
     * @param array    $options {
     *   @type int    maxRetries    Maximum number of retry attempts (default 3).
     *   @type int    initialDelay  Initial delay in milliseconds (default 500).
     *   @type float  multiplier    Delay multiplier per retry (default 2.0).
     *   @type int    maxDelay      Maximum delay in milliseconds (default 30000).
     *   @type bool   jitter        Add random jitter to avoid thundering herd (default true).
     * }
     * @throws Exception Last exception if all retries exhausted.
     * @return mixed Return value of the callable.
     */
    public static function withRetry(callable $callable, array $options = [])
    {
        $maxRetries   = (int)($options['maxRetries']   ?? 3);
        $initialDelay = (int)($options['initialDelay'] ?? 500);
        $multiplier   = (float)($options['multiplier'] ?? 2.0);
        $maxDelay     = (int)($options['maxDelay']     ?? 30000);
        $jitter       = (bool)($options['jitter']      ?? true);

        $lastException = null;
        $delay = $initialDelay;

        for ($attempt = 0; $attempt <= $maxRetries; $attempt++) {
            try {
                return $callable();
            } catch (SoapFault $e) {
                // Only retry transient SOAP faults; permanent faults (auth, invalid params) should not be retried
                if (!self::isTransientSoapFault($e)) {
                    throw $e;
                }
                $lastException = $e;
            } catch (Exception $e) {
                // Only retry on transient / connection-related errors
                if (!self::isTransient($e)) {
                    throw $e;
                }
                $lastException = $e;
            }

            if ($attempt < $maxRetries) {
                $sleepMs = $jitter ? (int)($delay * (0.8 + 0.4 * (mt_rand() / mt_getrandmax()))) : $delay;
                usleep(min($sleepMs, $maxDelay) * 1000);
                $delay = (int)min($delay * $multiplier, $maxDelay);
            }
        }

        throw new Exception(
            "Operation failed after {$maxRetries} retries. Last error: " . $lastException->getMessage(),
            $lastException->getCode(),
            $lastException
        );
    }

    /**
     * Determine whether an exception is likely a transient network/timeout error.
     */
    private static function isTransient(Exception $e): bool
    {
        $transientPhrases = [
            'connection',
            'timeout',
            'could not connect',
            'failed to connect',
            'network',
            'temporarily unavailable',
            'service unavailable',
            'ssl',
            'curl',
        ];
        $msg = strtolower($e->getMessage());
        foreach ($transientPhrases as $phrase) {
            if (strpos($msg, $phrase) !== false) {
                return true;
            }
        }
        return false;
    }

    /**
     * Determine whether a SoapFault is transient (connection/timeout) vs. permanent (auth, invalid params).
     *
     * Permanent fault codes include 'Client', 'VersionMismatch', 'MustUnderstand', 'DataEncodingUnknown'.
     * Server-side faults starting with 'Server' may be transient (upstream errors).
     */
    private static function isTransientSoapFault(SoapFault $e): bool
    {
        $permanentFaultCodes = ['Client', 'VersionMismatch', 'MustUnderstand', 'DataEncodingUnknown'];
        $faultCode = (string)$e->faultcode;

        // Permanent fault codes indicate the request itself is invalid; do not retry
        foreach ($permanentFaultCodes as $code) {
            if (stripos($faultCode, $code) !== false) {
                return false;
            }
        }

        // Authentication / authorization errors — permanent, do not retry
        $permanentPhrases = ['unauthorized', 'authentication', 'forbidden', 'access denied', 'invalid credentials'];
        $msg = strtolower($e->getMessage());
        foreach ($permanentPhrases as $phrase) {
            if (strpos($msg, $phrase) !== false) {
                return false;
            }
        }

        // Everything else (HTTP 5xx, network, timeouts) is considered transient
        return true;
    }
}
