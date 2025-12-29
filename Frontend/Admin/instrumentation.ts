/**
 * OpenTelemetry Instrumentation for B2Connect Frontend Admin
 *
 * This file configures distributed tracing and metrics for the Vite dev server.
 * It exports telemetry data to Aspire via OTLP protocol.
 *
 * Environment Variables:
 * - ENABLE_TELEMETRY: Set to 'true' to enable OpenTelemetry (default: false)
 * - OTEL_EXPORTER_OTLP_ENDPOINT: OTLP collector endpoint (default: http://localhost:4318)
 * - OTEL_SERVICE_NAME: Service name for traces (default: frontend-admin)
 *
 * Usage:
 * NODE_OPTIONS="--import ./instrumentation.ts" npm run dev
 *
 * @see https://opentelemetry.io/docs/languages/js/getting-started/nodejs/
 */

import { NodeSDK } from "@opentelemetry/sdk-node";
import { getNodeAutoInstrumentations } from "@opentelemetry/auto-instrumentations-node";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-proto";
import { OTLPMetricExporter } from "@opentelemetry/exporter-metrics-otlp-proto";
import { PeriodicExportingMetricReader } from "@opentelemetry/sdk-metrics";
import { Resource } from "@opentelemetry/resources";
import {
  ATTR_SERVICE_NAME,
  ATTR_SERVICE_VERSION,
  ATTR_DEPLOYMENT_ENVIRONMENT_NAME,
} from "@opentelemetry/semantic-conventions";

// Configuration with environment variable fallbacks
const config = {
  enabled: process.env.ENABLE_TELEMETRY === "true",
  serviceName: process.env.OTEL_SERVICE_NAME || "frontend-admin",
  serviceVersion: process.env.npm_package_version || "1.0.0",
  environment: process.env.NODE_ENV || "development",
  otlpEndpoint:
    process.env.OTEL_EXPORTER_OTLP_ENDPOINT || "http://localhost:4318",
};

/**
 * Initialize OpenTelemetry SDK with graceful fallback
 */
function initTelemetry(): void {
  // Skip initialization if telemetry is disabled
  if (!config.enabled) {
    console.log(
      "[OTel] Telemetry disabled. Set ENABLE_TELEMETRY=true to enable."
    );
    return;
  }

  console.log(`[OTel] Initializing telemetry for ${config.serviceName}...`);
  console.log(`[OTel] OTLP endpoint: ${config.otlpEndpoint}`);

  try {
    // Create resource with service attributes
    const resource = new Resource({
      [ATTR_SERVICE_NAME]: config.serviceName,
      [ATTR_SERVICE_VERSION]: config.serviceVersion,
      [ATTR_DEPLOYMENT_ENVIRONMENT_NAME]: config.environment,
    });

    // Configure OTLP trace exporter with timeout
    const traceExporter = new OTLPTraceExporter({
      url: `${config.otlpEndpoint}/v1/traces`,
      timeoutMillis: 5000, // 5 second timeout
    });

    // Configure OTLP metrics exporter
    const metricExporter = new OTLPMetricExporter({
      url: `${config.otlpEndpoint}/v1/metrics`,
      timeoutMillis: 5000,
    });

    // Configure metric reader with periodic export
    const metricReader = new PeriodicExportingMetricReader({
      exporter: metricExporter,
      exportIntervalMillis: 60000, // Export every 60 seconds
    });

    // Initialize the SDK
    const sdk = new NodeSDK({
      resource,
      traceExporter,
      metricReader,
      instrumentations: [
        getNodeAutoInstrumentations({
          // Disable noisy instrumentations
          "@opentelemetry/instrumentation-fs": {
            enabled: false, // Filesystem operations are too noisy for dev
          },
          "@opentelemetry/instrumentation-dns": {
            enabled: false, // DNS lookups are too frequent
          },
          // Enable HTTP instrumentation for API calls
          "@opentelemetry/instrumentation-http": {
            enabled: true,
          },
          // Enable fetch instrumentation
          "@opentelemetry/instrumentation-fetch": {
            enabled: true,
          },
        }),
      ],
    });

    // Start the SDK
    sdk.start();

    // Graceful shutdown on process termination
    const shutdown = async () => {
      console.log("[OTel] Shutting down telemetry...");
      try {
        await sdk.shutdown();
        console.log("[OTel] Telemetry shutdown complete.");
      } catch (error) {
        console.error("[OTel] Error during shutdown:", error);
      }
    };

    process.on("SIGTERM", shutdown);
    process.on("SIGINT", shutdown);

    console.log(`[OTel] Telemetry initialized successfully.`);
    console.log(`[OTel] Service: ${config.serviceName}@${config.serviceVersion}`);
    console.log(`[OTel] Environment: ${config.environment}`);
  } catch (error) {
    // Graceful fallback - don't crash if OTLP endpoint is unreachable
    console.warn("[OTel] Failed to initialize telemetry:", error);
    console.warn("[OTel] Continuing without telemetry...");
  }
}

// Initialize telemetry on module load
initTelemetry();
