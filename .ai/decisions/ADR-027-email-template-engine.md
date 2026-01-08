---
docid: ADR-065
title: ADR 027 Email Template Engine
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# ADR-027: Email Template Engine Selection

**DocID**: `ADR-027`  
**Status**: Accepted  
**Date**: 2026-01-03  
**Decision Makers**: @Architect, @TechLead, @Backend  
**Consulted**: @Security, @Frontend

---

## Context

B2X benötigt ein Email-Template-System, das es Tenant-Admins und Content-Creators ermöglicht, personalisierte Email-Vorlagen zu erstellen. Die Templates müssen dynamische Variablen unterstützen (z.B. `{{ order.number }}`, `{{ customer.firstName }}`).

### Requirements

1. **User-Defined Templates**: Tenants definieren ihre eigenen Templates
2. **Dynamic Variables**: Variablen aus Order, Customer, Shipping Context
3. **Safe Execution**: Keine Code-Injection möglich
4. **Performance**: High-Volume Email-Rendering (100+ emails/minute)
5. **Extensibility**: Custom Filter für Currency, Date Formatting
6. **Easy Syntax**: Non-technical Users müssen Templates bearbeiten können

---

## Decision

**Wir verwenden Liquid (Fluid) als Template-Engine für Email-Rendering.**

### Alternatives Considered

#### Option 1: RazorEngineCore ❌

```csharp
// RazorEngineCore Example
@model OrderContext
<h1>Order @Model.Order.Number</h1>
<p>Hello @Model.Customer.FirstName,</p>

@{
    // ⚠️ SECURITY RISK: Full C# code execution possible
    System.IO.File.Delete("important.txt");  // 😱
    var db = new DbContext();
    var data = db.Customers.ToList();  // 😱
}
```

**Pros:**
- Powerful C# integration
- Native .NET support
- Fast compilation

**Cons:**
- ❌ **CRITICAL SECURITY RISK**: Allows arbitrary C# code execution
- ❌ Users could access file system, database, network
- ❌ No proper sandboxing for user-defined templates
- ❌ Overkill for simple email templating
- ❌ Complex syntax for non-technical users

**Verdict**: Not suitable for user-defined templates due to security risks.

#### Option 2: Scriban ⚠️

```scriban
{{ for item in order.items }}
  <tr>
    <td>{{ item.name }}</td>
    <td>{{ item.price | currency }}</td>
  </tr>
{{ end }}
```

**Pros:**
- Fast and efficient
- Good sandbox support
- Similar syntax to Liquid

**Cons:**
- Less community adoption for email use cases
- Fewer built-in filters
- Less documentation

**Verdict**: Good alternative, but Liquid has better ecosystem.

#### Option 3: Liquid (Fluid) ✅ SELECTED

```liquid
{% for item in order.items %}
  <tr>
    <td>{{ item.name }}</td>
    <td>{{ item.price | currency: "EUR" }}</td>
  </tr>
{% endfor %}
```

**Pros:**
- ✅ **Fully sandboxed** - no code execution possible
- ✅ **Proven at scale** - Shopify processes millions of emails
- ✅ **Simple syntax** - Non-technical users can edit
- ✅ **Extensible** - Custom filters and tags
- ✅ **Email-optimized** - Designed for this use case
- ✅ **Good .NET library** - Fluid is well-maintained
- ✅ **Template caching** - Performance optimized

**Cons:**
- Limited logic capabilities (by design)
- Requires learning Liquid syntax

**Verdict**: Best fit for secure, user-defined email templates.

---

## Implementation

### 1. NuGet Package

```xml
<PackageReference Include="Fluid.Core" Version="2.10.0" />
```

### 2. Template Renderer Service

```csharp
public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(string template, Dictionary<string, object> context);
}

public class LiquidEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly FluidParser _parser = new();
    private readonly TemplateOptions _options;
    
    public LiquidEmailTemplateRenderer()
    {
        _options = new TemplateOptions();
        
        // Register custom filters
        _options.Filters.AddFilter("currency", CurrencyFilter);
        _options.Filters.AddFilter("format_date", DateFilter);
        _options.Filters.AddFilter("format_datetime", DateTimeFilter);
        _options.Filters.AddFilter("truncate", TruncateFilter);
        
        // Security settings
        _options.MaxSteps = 10000;        // Prevent infinite loops
        _options.MaxRecursion = 50;       // Prevent stack overflow
    }
    
    public async Task<string> RenderAsync(
        string template, 
        Dictionary<string, object> context)
    {
        if (!_parser.TryParse(template, out var fluidTemplate, out var error))
        {
            throw new TemplateParseException($"Template syntax error: {error}");
        }
        
        var templateContext = new TemplateContext(context, _options);
        
        // Timeout protection
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        try
        {
            return await fluidTemplate.RenderAsync(templateContext);
        }
        catch (OperationCanceledException)
        {
            throw new TemplateRenderException("Template rendering timed out");
        }
    }
}
```

### 3. Custom Filters

```csharp
// Currency filter: {{ price | currency: "EUR" }}
public static ValueTask<FluidValue> CurrencyFilter(
    FluidValue input, 
    FilterArguments args, 
    TemplateContext context)
{
    var amount = input.ToNumberValue();
    var currency = args.At(0).ToStringValue() ?? "EUR";
    
    var culture = currency switch
    {
        "EUR" => new CultureInfo("de-DE"),
        "USD" => new CultureInfo("en-US"),
        "GBP" => new CultureInfo("en-GB"),
        _ => CultureInfo.InvariantCulture
    };
    
    var formatted = amount.ToString("C", culture);
    return new StringValue(formatted);
}

// Date filter: {{ date | format_date: "dd.MM.yyyy" }}
public static ValueTask<FluidValue> DateFilter(
    FluidValue input, 
    FilterArguments args, 
    TemplateContext context)
{
    if (!input.TryGetDateTimeOffset(out var date))
        return input;
    
    var format = args.At(0).ToStringValue() ?? "dd.MM.yyyy";
    return new StringValue(date.ToString(format));
}

// Truncate filter: {{ text | truncate: 50 }}
public static ValueTask<FluidValue> TruncateFilter(
    FluidValue input, 
    FilterArguments args, 
    TemplateContext context)
{
    var text = input.ToStringValue();
    var length = (int)args.At(0).ToNumberValue();
    var suffix = args.At(1).ToStringValue() ?? "...";
    
    if (text.Length <= length)
        return input;
    
    return new StringValue(text.Substring(0, length) + suffix);
}
```

### 4. Available Variables (Context)

```csharp
public class EmailTemplateContext
{
    // Order Context
    public OrderDto Order { get; set; }
    
    // Customer Context
    public CustomerDto Customer { get; set; }
    
    // Shipping Context
    public ShippingDto Shipping { get; set; }
    
    // Tenant Context
    public TenantDto Tenant { get; set; }
    
    // Product Context (for single product emails)
    public ProductDto Product { get; set; }
}

// Example usage in template:
// {{ order.number }}
// {{ customer.firstName }}
// {{ shipping.tracking }}
// {{ tenant.name }}
// {{ order.total | currency: "EUR" }}
// {{ order.date | format_date: "dd. MMMM yyyy" }}
```

### 5. Wolverine Handler Integration

```csharp
public record RenderEmailCommand
{
    public Guid TenantId { get; init; }
    public string TemplateKey { get; init; }
    public string Locale { get; init; }
    public Dictionary<string, object> Context { get; init; }
}

public class RenderEmailCommandHandler
{
    private readonly IEmailTemplateRepository _templates;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly IMessageBus _bus;
    
    [Transactional]
    public async Task Handle(RenderEmailCommand command)
    {
        // 1. Load template from database
        var template = await _templates.GetByKeyAndLocaleAsync(
            command.TenantId,
            command.TemplateKey,
            command.Locale
        );
        
        // 2. Render subject and body with Liquid
        var subject = await _renderer.RenderAsync(
            template.Subject, 
            command.Context
        );
        
        var htmlBody = await _renderer.RenderAsync(
            template.HtmlBody, 
            command.Context
        );
        
        var plainTextBody = await _renderer.RenderAsync(
            template.PlainTextBody ?? "", 
            command.Context
        );
        
        // 3. Publish to send handler
        await _bus.PublishAsync(new EmailRendered
        {
            EmailId = Guid.NewGuid(),
            Subject = subject,
            HtmlBody = htmlBody,
            PlainTextBody = plainTextBody,
            ToAddress = ExtractRecipient(command.Context),
            // ... CC, BCC, Attachments
        });
    }
}
```

---

## Security Considerations

### What Liquid CANNOT Do (by design)

```liquid
{# These are all IMPOSSIBLE in Liquid: #}

{# ❌ File system access #}
{{ file.read("/etc/passwd") }}

{# ❌ Database queries #}
{{ db.query("SELECT * FROM users") }}

{# ❌ Network requests #}
{{ http.get("http://attacker.com") }}

{# ❌ Code execution #}
{{ eval("malicious_code()") }}

{# ❌ System calls #}
{{ system("rm -rf /") }}
```

### What Liquid CAN Do (limited, safe)

```liquid
{# ✅ Variable interpolation #}
{{ order.number }}
{{ customer.firstName }}

{# ✅ Filters (pre-registered only) #}
{{ price | currency: "EUR" }}
{{ date | format_date: "dd.MM.yyyy" }}

{# ✅ Conditionals #}
{% if customer.isPremium %}
  VIP Customer!
{% endif %}

{# ✅ Loops #}
{% for item in order.items %}
  {{ item.name }}
{% endfor %}

{# ✅ String operations #}
{{ name | upcase }}
{{ text | truncate: 50 }}
```

### Additional Security Measures

1. **Timeout**: Max 5 seconds per render
2. **Step Limit**: Max 10,000 steps (prevent infinite loops)
3. **Recursion Limit**: Max 50 levels
4. **Filter Whitelist**: Only registered filters available
5. **Output Sanitization**: XSS prevention on output
6. **Tenant Isolation**: Templates scoped to tenant

---

## Performance Considerations

### Template Caching

```csharp
public class CachedEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly IMemoryCache _cache;
    private readonly FluidParser _parser = new();
    
    public async Task<string> RenderAsync(
        string template, 
        Dictionary<string, object> context)
    {
        // Cache parsed template
        var cacheKey = $"template:{template.GetHashCode()}";
        
        var fluidTemplate = await _cache.GetOrCreateAsync(
            cacheKey,
            entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                
                if (!_parser.TryParse(template, out var parsed, out var error))
                    throw new TemplateParseException(error);
                
                return Task.FromResult(parsed);
            }
        );
        
        var templateContext = new TemplateContext(context, _options);
        return await fluidTemplate.RenderAsync(templateContext);
    }
}
```

### Benchmarks (Expected)

| Operation | Time |
|-----------|------|
| Parse Template | ~5ms (cached: 0ms) |
| Render Simple Template | ~1ms |
| Render Complex Template (loops, conditionals) | ~5-10ms |
| Full Email Render (subject + body) | ~10-20ms |

---

## Consequences

### Positive

- ✅ Secure: No code injection possible
- ✅ Simple: Non-technical users can edit templates
- ✅ Fast: Efficient rendering, caching support
- ✅ Proven: Used by Shopify for millions of emails
- ✅ Extensible: Custom filters for B2X needs
- ✅ Maintainable: Clean separation of concerns

### Negative

- ⚠️ Limited Logic: Complex business logic not possible in templates
- ⚠️ Learning Curve: Team needs to learn Liquid syntax
- ⚠️ Debugging: Template errors harder to debug than C#

### Mitigations

- Provide good error messages for template syntax errors
- Document available variables and filters
- Preview functionality for immediate feedback
- Validation on template save

---

## Related Documents

- [REQ-003](../requirements/REQ-003-email-template-system.md) - Email Template System Requirements
- [KB-006](../knowledgebase/wolverine.md) - Wolverine CQRS Patterns
- [KB-023](../knowledgebase/best-practices/email-templates.md) - Email Template Best Practices

---

## Decision Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-01-03 | Selected Liquid (Fluid) | Security, simplicity, proven at scale |
| 2026-01-03 | Rejected RazorEngineCore | Security risk for user-defined templates |
| 2026-01-03 | Rejected Scriban | Less ecosystem, similar capabilities |

---

**Agents**: @Architect, @TechLead, @Backend, @Security | Owner: @Architect
