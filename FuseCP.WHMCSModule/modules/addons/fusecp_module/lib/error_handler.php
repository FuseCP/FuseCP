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
        // SoapFaults from network issues have faultcode starting with HTTP or Client
        if ($e instanceof SoapFault) {
            return true;
        }
        return false;
    }
}
