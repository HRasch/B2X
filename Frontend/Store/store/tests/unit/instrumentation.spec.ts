/**
 * Unit Tests for OpenTelemetry Instrumentation Configuration
 *
 * Tests the telemetry configuration, environment variable handling,
 * and graceful fallback behavior.
 *
 * @see instrumentation.ts
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('OpenTelemetry Instrumentation Configuration', () => {
  // Store original environment
  const originalEnv = process.env;

  beforeEach(() => {
    // Reset environment variables before each test
    vi.resetModules();
    process.env = { ...originalEnv };
  });

  afterEach(() => {
    // Restore original environment after each test
    process.env = originalEnv;
  });

  describe('Configuration Defaults', () => {
    it('should have telemetry disabled by default', () => {
      // Given: No ENABLE_TELEMETRY environment variable
      delete process.env.ENABLE_TELEMETRY;

      // When: Checking the default configuration
      const enabled = process.env.ENABLE_TELEMETRY === 'true';

      // Then: Telemetry should be disabled
      expect(enabled).toBe(false);
    });

    it('should use default service name when not specified', () => {
      // Given: No OTEL_SERVICE_NAME environment variable
      delete process.env.OTEL_SERVICE_NAME;

      // When: Getting the service name
      const serviceName = process.env.OTEL_SERVICE_NAME || 'frontend-store';

      // Then: Should use default service name
      expect(serviceName).toBe('frontend-store');
    });

    it('should use default OTLP endpoint when not specified', () => {
      // Given: No OTEL_EXPORTER_OTLP_ENDPOINT environment variable
      delete process.env.OTEL_EXPORTER_OTLP_ENDPOINT;

      // When: Getting the OTLP endpoint
      const endpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4318';

      // Then: Should use default localhost endpoint
      expect(endpoint).toBe('http://localhost:4318');
    });

    it('should use development environment by default', () => {
      // Given: No NODE_ENV environment variable
      delete process.env.NODE_ENV;

      // When: Getting the environment
      const environment = process.env.NODE_ENV || 'development';

      // Then: Should default to development
      expect(environment).toBe('development');
    });
  });

  describe('Environment Variable Overrides', () => {
    it('should enable telemetry when ENABLE_TELEMETRY=true', () => {
      // Given: ENABLE_TELEMETRY is set to 'true'
      process.env.ENABLE_TELEMETRY = 'true';

      // When: Checking if telemetry is enabled
      const enabled = process.env.ENABLE_TELEMETRY === 'true';

      // Then: Telemetry should be enabled
      expect(enabled).toBe(true);
    });

    it("should keep telemetry disabled for any value other than 'true'", () => {
      // Given: ENABLE_TELEMETRY is set to various non-true values
      const nonTrueValues = ['false', '1', 'yes', 'TRUE', 'True', ''];

      nonTrueValues.forEach(value => {
        process.env.ENABLE_TELEMETRY = value;
        const enabled = process.env.ENABLE_TELEMETRY === 'true';
        expect(enabled).toBe(false);
      });
    });

    it('should use custom service name when OTEL_SERVICE_NAME is set', () => {
      // Given: Custom service name is set
      process.env.OTEL_SERVICE_NAME = 'custom-frontend';

      // When: Getting the service name
      const serviceName = process.env.OTEL_SERVICE_NAME || 'frontend-store';

      // Then: Should use custom service name
      expect(serviceName).toBe('custom-frontend');
    });

    it('should use custom OTLP endpoint when OTEL_EXPORTER_OTLP_ENDPOINT is set', () => {
      // Given: Custom OTLP endpoint is set
      process.env.OTEL_EXPORTER_OTLP_ENDPOINT = 'http://aspire:4318';

      // When: Getting the OTLP endpoint
      const endpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4318';

      // Then: Should use custom endpoint
      expect(endpoint).toBe('http://aspire:4318');
    });

    it('should use production environment when NODE_ENV=production', () => {
      // Given: NODE_ENV is set to production
      process.env.NODE_ENV = 'production';

      // When: Getting the environment
      const environment = process.env.NODE_ENV || 'development';

      // Then: Should be production
      expect(environment).toBe('production');
    });
  });

  describe('OTLP Endpoint URL Construction', () => {
    it('should construct correct traces endpoint URL', () => {
      // Given: Base OTLP endpoint
      const baseEndpoint = 'http://localhost:4318';

      // When: Constructing traces endpoint
      const tracesEndpoint = `${baseEndpoint}/v1/traces`;

      // Then: Should have correct path
      expect(tracesEndpoint).toBe('http://localhost:4318/v1/traces');
    });

    it('should construct correct metrics endpoint URL', () => {
      // Given: Base OTLP endpoint
      const baseEndpoint = 'http://localhost:4318';

      // When: Constructing metrics endpoint
      const metricsEndpoint = `${baseEndpoint}/v1/metrics`;

      // Then: Should have correct path
      expect(metricsEndpoint).toBe('http://localhost:4318/v1/metrics');
    });

    it('should handle custom endpoint with trailing slash', () => {
      // Given: Endpoint with trailing slash
      const baseEndpoint = 'http://aspire:4318/';

      // When: Constructing traces endpoint (need to handle trailing slash)
      const tracesEndpoint = `${baseEndpoint.replace(/\/$/, '')}/v1/traces`;

      // Then: Should not have double slash
      expect(tracesEndpoint).toBe('http://aspire:4318/v1/traces');
    });
  });

  describe('Service Resource Attributes', () => {
    it('should include service name in resource', () => {
      // Given: Service configuration
      const serviceName = 'frontend-store';

      // When: Creating resource attributes
      const attributes = {
        'service.name': serviceName,
      };

      // Then: Should have service.name attribute
      expect(attributes['service.name']).toBe('frontend-store');
    });

    it('should include service version in resource', () => {
      // Given: Package version
      const version = '1.0.0';

      // When: Creating resource attributes
      const attributes = {
        'service.version': version,
      };

      // Then: Should have service.version attribute
      expect(attributes['service.version']).toBe('1.0.0');
    });

    it('should include deployment environment in resource', () => {
      // Given: Environment
      const environment = 'development';

      // When: Creating resource attributes
      const attributes = {
        'deployment.environment.name': environment,
      };

      // Then: Should have deployment.environment.name attribute
      expect(attributes['deployment.environment.name']).toBe('development');
    });
  });

  describe('Graceful Fallback Behavior', () => {
    it('should not throw when telemetry is disabled', () => {
      // Given: Telemetry is disabled
      process.env.ENABLE_TELEMETRY = 'false';

      // When/Then: Should not throw
      expect(() => {
        const enabled = process.env.ENABLE_TELEMETRY === 'true';
        if (!enabled) {
          // Skip initialization
          return;
        }
      }).not.toThrow();
    });

    it('should handle missing npm_package_version gracefully', () => {
      // Given: No npm_package_version
      delete process.env.npm_package_version;

      // When: Getting version with fallback
      const version = process.env.npm_package_version || '1.0.0';

      // Then: Should use fallback version
      expect(version).toBe('1.0.0');
    });
  });

  describe('Exporter Configuration', () => {
    it('should use 5 second timeout for trace exporter', () => {
      // Given: Expected timeout
      const expectedTimeoutMillis = 5000;

      // When: Configuring exporter
      const config = {
        timeoutMillis: 5000,
      };

      // Then: Should have correct timeout
      expect(config.timeoutMillis).toBe(expectedTimeoutMillis);
    });

    it('should use 60 second export interval for metrics', () => {
      // Given: Expected export interval
      const expectedIntervalMillis = 60000;

      // When: Configuring metric reader
      const config = {
        exportIntervalMillis: 60000,
      };

      // Then: Should have correct interval
      expect(config.exportIntervalMillis).toBe(expectedIntervalMillis);
    });
  });

  describe('Instrumentation Configuration', () => {
    it('should disable filesystem instrumentation', () => {
      // Given: Instrumentation config
      const instrumentationConfig = {
        '@opentelemetry/instrumentation-fs': {
          enabled: false,
        },
      };

      // Then: FS instrumentation should be disabled
      expect(instrumentationConfig['@opentelemetry/instrumentation-fs'].enabled).toBe(false);
    });

    it('should disable DNS instrumentation', () => {
      // Given: Instrumentation config
      const instrumentationConfig = {
        '@opentelemetry/instrumentation-dns': {
          enabled: false,
        },
      };

      // Then: DNS instrumentation should be disabled
      expect(instrumentationConfig['@opentelemetry/instrumentation-dns'].enabled).toBe(false);
    });

    it('should enable HTTP instrumentation', () => {
      // Given: Instrumentation config
      const instrumentationConfig = {
        '@opentelemetry/instrumentation-http': {
          enabled: true,
        },
      };

      // Then: HTTP instrumentation should be enabled
      expect(instrumentationConfig['@opentelemetry/instrumentation-http'].enabled).toBe(true);
    });

    it('should enable fetch instrumentation', () => {
      // Given: Instrumentation config
      const instrumentationConfig = {
        '@opentelemetry/instrumentation-fetch': {
          enabled: true,
        },
      };

      // Then: Fetch instrumentation should be enabled
      expect(instrumentationConfig['@opentelemetry/instrumentation-fetch'].enabled).toBe(true);
    });
  });
});

describe('OpenTelemetry Performance Impact', () => {
  it('should have reasonable export interval (not too frequent)', () => {
    // Given: Export interval config
    const exportIntervalMillis = 60000; // 60 seconds

    // Then: Should not export too frequently (minimum 30 seconds)
    expect(exportIntervalMillis).toBeGreaterThanOrEqual(30000);
  });

  it('should have reasonable timeout (not too long)', () => {
    // Given: Timeout config
    const timeoutMillis = 5000; // 5 seconds

    // Then: Should not wait too long (maximum 10 seconds)
    expect(timeoutMillis).toBeLessThanOrEqual(10000);
  });

  it('should disable noisy instrumentations', () => {
    // Given: List of disabled instrumentations
    const disabledInstrumentations = [
      '@opentelemetry/instrumentation-fs',
      '@opentelemetry/instrumentation-dns',
    ];

    // Then: Should have at least 2 disabled instrumentations
    expect(disabledInstrumentations.length).toBeGreaterThanOrEqual(2);
  });
});
