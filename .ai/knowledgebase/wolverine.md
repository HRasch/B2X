
Title: Wolverine (JasperFx) — messaging & CQRS overview
Source: https://github.com/JasperFx/wolverine, https://wolverinefx.net/

Summary:
- Wolverine is a next-generation .NET mediator and message bus that unifies in-process mediation, durable messaging, and transport integrations. It targets CQRS and event-driven patterns and is often paired with Marten (document DB) in the "critter stack."
- Key capabilities: in-process message routing (mediator-style), durable transports (RabbitMQ, Azure Service Bus, Kafka via adapters), durable node/durability features (outbox, retries, scheduling), message persistence integrations, idempotent handlers, and an HTTP transport.

Important notes:
- Documentation is maintained in the repo `/docs` and published via VitePress; you can run the docs locally with `npm install` and `npm run docs` per the README.
- Active project: frequent releases (v5.x series), community discussions, and paid support options from JasperFx.

Best practices / Actionables for this repo:
- Identify any direct references to `Wolverine` packages in the codebase and map the message handler types to bounded contexts.
- Use outbox/durability for operations that need transactional guarantees between DB and messages.
- Prefer idempotent handlers and design message contracts to be versionable.

References & links:
- Repo: https://github.com/JasperFx/wolverine
- Official site / docs landing: https://wolverinefx.net/
- README (raw): https://raw.githubusercontent.com/jasperfx/wolverine/main/README.md

