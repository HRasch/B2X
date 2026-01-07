# B2X — Technical Requirements (for @TechLead)

Purpose: concrete technical constraints and design decisions to guide implementation and reviews.

1. API Specifications
   - Expose REST + GraphQL (read) in MVP; plan GraphQL write support in P2. Provide OpenAPI docs and GraphQL playground.
   - Use stable contract testing for API consumers.

2. Authentication & Authorization
   - JWT and API Keys for service-to-service; OAuth2 for third-party apps. RBAC for admin and operator roles.

3. Data Storage
   - Relational DB (Postgres compatible) as source of truth for catalog/orders; Redis for caches and sessions.

4. Search
   - Use Elasticsearch/Opensearch for faceted search; maintain sync pipeline from canonical DB.

5. Messaging & Events
   - Use durable message broker (e.g., Kafka/RabbitMQ) for event-driven flows and integrations. Events must be idempotent and versioned.

6. Webhooks
   - At-least-once delivery with exponential backoff, signed payloads, and dead-letter handling.

7. Import/Export
   - CSV first; adapter hooks for BMECat/DATANORM; validation engine with actionable errors.

8. Image/Media Pipeline
   - Upload to object storage; generate AVIF/WebP/PNG variants; integrate CDN with signed URLs and cache-control.

9. Observability
   - Structured JSON logs, Prometheus metrics, traces (OpenTelemetry), and a centralized logging pipeline.

10. CI/CD & Testing
   - Automated pipelines for build/test/deploy; unit, integration, contract and e2e tests; staging and canary deployments.

11. Secrets & Config
   - Use secrets manager (HashiCorp Vault / cloud provider KMS) and environment-based configuration with feature flags.

12. Resilience & Backups
   - Circuit breakers, retries with jitter, idempotency tokens for operations; scheduled backups and point-in-time recovery where supported.

13. Compliance Controls
   - Encryption at rest and in transit; audit trails; PCI assessment plan for payment flows.

14. Rate Limiting & Quotas
   - Per-tenant quotas and throttling; support tiered limits.

15. Deployment Targets
   - Containerized (Docker), orchestrated on Kubernetes; infra-as-code for cloud provisioning.
