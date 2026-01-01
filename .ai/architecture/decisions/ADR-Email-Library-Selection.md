# ADR: Email Sending Library Selection

## Status
✅ Approved - 31. Dezember 2025

## Context
The B2Connect project requires a robust, cross-platform email sending capability for the Email domain. The system needs to support SMTP with modern security features, async operations, and compliance with RFC standards.

## Decision
We will use **MailKit** as the email sending library for the following reasons:

- **Modern and RFC-compliant**: Full support for SMTP, IMAP, and POP3 protocols with proper RFC adherence
- **Cross-platform**: Works on .NET Core, .NET Framework, and .NET 5+
- **Security**: Built-in support for SSL/TLS, SASL authentication, and modern encryption
- **Performance**: Async/await support, connection pooling, and optimized for high-volume email sending
- **Active maintenance**: Regularly updated with 196M+ NuGet downloads
- **Microsoft recommendation**: Officially recommended replacement for deprecated System.Net.Mail.SmtpClient

## Alternatives Considered
- **System.Net.Mail.SmtpClient**: Deprecated by Microsoft, lacks modern features
- **SendGrid/AWS SES/Azure Communication Email**: Cloud services requiring API keys and costs
- **FluentEmail**: Wrapper library that can use MailKit underneath

## Consequences
- **Positive**: Robust, secure, and performant email sending
- **Negative**: Additional NuGet dependency (MailKit)
- **Implementation**: @Backend will integrate MailKit into the Email domain's IEmailProvider implementation

## References
- [MailKit GitHub](https://github.com/jstedfast/MailKit)
- [MailKit NuGet](https://www.nuget.org/packages/MailKit/)
- [Microsoft SmtpClient Deprecation Notice](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient)

## Responsible
@Architect - Decision Maker
@Backend - Implementation</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/ADR-Email-Library-Selection.md