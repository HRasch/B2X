---
docid: KB-192
title: Wolverine
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

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

## Basic Concepts and Terminology (from https://wolverinefx.net/guide/basics.html)

### Core Concepts

- **Message**: A .NET class, struct, or record that represents either a command (triggering an operation) or event (notifying other parts of the system). Messages are serializable and have no structural differentiation in Wolverine.
- **Message Handler**: A method that processes an incoming message. Handlers can be static methods or instance methods on classes.
- **Envelope**: Wolverine's wrapper around messages containing metadata like headers, correlation IDs, and delivery information.

### Architecture Components

- **Transport**: Wolverine's support for external messaging infrastructure (RabbitMQ, Amazon SQS, Azure Service Bus, TCP transport).
- **Endpoint**: Configuration for a Wolverine connection to an external resource (e.g., RabbitMQ exchange, SQS queue). May align with AsyncAPI "channel" terminology in future versions.
- **Sending Agent**: Internal Wolverine component that publishes outgoing messages to transport endpoints.
- **Listening Agent**: Internal component that receives messages from external transports and routes them to message handlers.

### Runtime Concepts

- **Node**: A running instance of a Wolverine application within a cluster. Not to be confused with Node.js or Kubernetes nodes.
- **Agent**: Stateful software agents that run on a single node, with Wolverine managing their distribution. Mostly internal but affects clustering behavior.
- **Message Store**: Database storage for Wolverine's inbox/outbox persistent messaging. Required for leader election, node assignments, durable scheduling, and transactional messaging.
- **Durability Agent**: Background service that interacts with the message store for transactional inbox/outbox functionality.

### Usage Patterns

- **Local Mediator**: Wolverine can act as an in-process command bus, allowing code to invoke message handlers without knowing implementation details using `IMessageBus.InvokeAsync()`.
- **External Messaging**: Wolverine supports publishing and processing messages through external infrastructure like RabbitMQ, Pulsar, or other transports.
- **HTTP Transport**: Wolverine treats HTTP requests as a specialized form of messaging, enabling unified handling of commands/events across protocols.

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

- Wolverine 3.0+ is more tolerant of shutdowns, making it less critical to worry about graceful shutdowns compared to earlier versions.
- Wolverine operates in `Balanced` mode by default, enabling it to function as a cluster of nodes. Ideally, node processes should be gracefully shut down to prevent communication failures with other nodes. A health check process identifies stale nodes.
- When running Wolverine in containers, ensure the orchestrator sends proper TERM signals and allows sufficient time before forcefully killing the process. Reference Kubernetes [Pod termination](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/#pod-termination) for best practices.

## CancellationToken Support in Message Handlers

Wolverine supports `CancellationToken` as a method parameter in message handlers for handling timeouts and system shutdown scenarios:

### Handler Parameter Injection

- **CancellationToken**: Accept `CancellationToken` as a method parameter to check for timeouts or system shutdown
- **Usage**: Inject directly into handler methods alongside other dependencies
- **Purpose**: Enables graceful cancellation of long-running operations and proper shutdown handling

### Example Usage

```csharp
public static class OrderProcessingHandler
{
    public static async Task Handle(
        ProcessOrder command,
        CancellationToken cancellationToken,
        IDocumentSession session)
    {
        // Check for cancellation before starting expensive operations
        cancellationToken.ThrowIfCancellationRequested();

        // Use token with async operations
        var order = await session.LoadAsync<Order>(command.OrderId, cancellationToken);

        // Long-running business logic with cancellation support
        await ProcessOrderAsync(order, cancellationToken);
    }
}
```

### Best Practices for CancellationToken

- **Check early**: Validate cancellation state before expensive operations
- **Propagate token**: Pass the token to all async operations that support it
- **Handle gracefully**: Use `ThrowIfCancellationRequested()` or check `IsCancellationRequested`
- **Avoid blocking**: Don't perform blocking operations that can't be cancelled
- **Testing**: Test cancellation scenarios to ensure proper cleanup

### Integration with Timeouts

- Combine with Wolverine's [execution timeout](https://wolverinefx.net/guide/handlers/timeout) configuration
- CancellationToken will be signalled when timeout is exceeded
- Enables proper resource cleanup on timeout scenarios

References & links:

- Repo: https://github.com/JasperFx/wolverine
- Official site / docs landing: https://wolverinefx.net/
- README (raw): https://raw.githubusercontent.com/jasperfx/wolverine/main/README.md
- Latest release: https://github.com/JasperFx/wolverine/releases/tag/V5.9.1
