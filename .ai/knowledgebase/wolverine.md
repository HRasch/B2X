
Title: Wolverine (JasperFx) — messaging & CQRS overview
Source: https://github.com/JasperFx/wolverine, https://wolverinefx.net/

Summary:
- Wolverine is a next-generation .NET mediator and message bus that unifies in-process mediation, durable messaging, and transport integrations. It targets CQRS and event-driven patterns and is often paired with Marten (document DB) in the "critter stack."
- Key capabilities: in-process message routing (mediator-style), durable transports (RabbitMQ, Azure Service Bus, Kafka via adapters), durable node/durability features (outbox, retries, scheduling), message persistence integrations, idempotent handlers, and an HTTP transport.
- Latest version: 5.9.1 (released 2 weeks ago as of Jan 2026), with features like Vapor Mode (opt-in compilation mode for reduced bundle size and improved performance), HTTP transport support, and enhanced CloudEvents handling.

Important notes:
- Documentation is maintained in the repo `/docs` and published via VitePress; you can run the docs locally with `npm install` and `npm run docs` per the README.
- Active project: frequent releases (v5.x series), community discussions, and paid support options from JasperFx.
- Vapor Mode: New compilation mode in v3.6+ for better performance, opt-in with `<script setup vapor>`.

Best practices / Actionables for this repo:
- Identify any direct references to `Wolverine` packages in the codebase and map the message handler types to bounded contexts.
- Use outbox/durability for operations that need transactional guarantees between DB and messages.
- Prefer idempotent handlers and design message contracts to be versionable.
- Consider Vapor Mode for performance-critical components (requires `<script setup>`).

## Wolverine Best Practices (from https://wolverinefx.net/introduction/best-practices.html)

### Dividing Handlers
- Wolverine does not enforce explicit assignment of handlers to endpoints, but you may want to define routing for throughput, parallelization, or message ordering.
- Prefer single message handler methods per class for better results, especially for complex handlers.

### Avoid Abstracting Wolverine
- Do not abstract Wolverine; it reduces functionality. Use cascading messages for testability instead.

### Lean on Wolverine Error Handling
- Use Wolverine's configurable error handling policies instead of explicit exception catching for better observability and cleaner code.

### Pre-Generate Types to Optimize Production Usage
- Use pre-generated types to avoid memory usage and cold start issues. See [Working with Code Generation](https://wolverinefx.net/guide/codegen).

### Prefer Pure Functions for Business Logic
- Use pure functions for business logic to enable easy unit testing without mocks.
- See related articles: [A-Frame Architecture with Wolverine](https://jeremydmiller.com/2023/07/19/a-frame-architecture-with-wolverine/), [Testing Without Mocks](https://www.jamesshore.com/v2/projects/nullables/testing-without-mocks), [Compound Handlers](https://jeremydmiller.com/2023/03/07/compound-handlers-in-wolverine/), [Isolating Side Effects](https://jeremydmiller.com/2023/04/24/isolating-side-effects-from-wolverine-handlers/).

### Make All Side Effects Apparent from the Root Message Handler
- Publish cascading messages from the root handler to make side effects visible and code easier to reason about.
- Avoid publishing messages deep in the call stack.

### Keep Your Call Stacks Short
- Maintain short call stacks for easier reasoning. Prefer A-Frame Architecture over heavy layering.

### Attaining IMessageBus
- Inject `IMessageBus` as a method parameter or constructor dependency.
- Avoid resolving via scoped containers to prevent state issues.

### IoC Container Usage
- Wolverine avoids IoC at runtime; prefer method injection over constructor injection.
- Avoid opaque service registrations that require runtime resolution.
- Inspect generated code for service scope issues.

### Vertical Slice Architecture
- Wolverine supports vertical slice organization for maintainability with less complexity than layered architectures.
- See [Low Ceremony Vertical Slice Architecture with Wolverine](https://jeremydmiller.com/2023/07/10/low-ceremony-vertical-slice-architecture-with-wolverine/).

### Graceful Shutdown of Nodes
- Wolverine 3.0+ is more tolerant of shutdowns. Ensure proper TERM signals in containers.

References & links:
- Repo: https://github.com/JasperFx/wolverine
- Official site / docs landing: https://wolverinefx.net/
- README (raw): https://raw.githubusercontent.com/jasperfx/wolverine/main/README.md
- Latest release: https://github.com/JasperFx/wolverine/releases/tag/V5.9.1

