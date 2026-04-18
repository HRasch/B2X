# B2X.Notifications

This folder contains the Notifications gateway service. It exposes a simple HTTP API for sending notifications and will host adapters for email, push, SMS and WhatsApp.

Next steps:
- Wire adapters (per-channel) under `src/backend/Notifications/Adapters`
- Add ProjectReferences to shared messaging / domain projects as needed
- Configure reverse-proxy rules in gateways (Admin/Management) to route `/api/notifications` here

Development quick checks
----------------------

1) Start the Notifications service locally (port 5009):

```powershell
dotnet run --project src/backend/Notifications/Gateway/B2X.Notifications.csproj
```

2) Health check (direct):

```bash
curl -i http://localhost:5009/health
```

3) Send notification directly (example - broadcasts to all no-op adapters):

```bash
curl -i -X POST http://localhost:5009/api/notifications/send \
	-H "Content-Type: application/json" \
	-d '{"Type":"broadcast","TenantId":"tenant-1","Payload":{"message":"hello"}}'
```

4) Send notification via Management gateway (proxy):

```bash
curl -i -X POST http://localhost:8000/api/notifications/send \
	-H "Content-Type: application/json" \
	-d '{"Type":"email","TenantId":"tenant-1","Payload":{"subject":"hi","body":"hello"}}'
```

Replace ports/hosts according to your local dev env. The Management gateway development config routes `/api/notifications` to `http://localhost:5009/`.
