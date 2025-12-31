# Mermaid Diagrams in VS Code & GitHub

**Version:** 1.0 | **Last Updated:** 30. Dezember 2025

Mermaid is a JavaScript-based diagramming and charting tool that uses a simple, intuitive syntax to create diagrams. It's natively supported in GitHub and VS Code markdown.

---

## 📋 Table of Contents

1. [Support & Platforms](#support--platforms)
2. [Diagram Types](#diagram-types)
3. [Basic Syntax](#basic-syntax)
4. [Common Diagrams](#common-diagrams)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)

---

## Support & Platforms

### VS Code
- **Built-in Support**: Markdown Preview includes Mermaid rendering
- **Enable**: File → Preferences → Settings → Search "markdown.mermaid" → Ensure `true`
- **Preview**: Click the Eye icon in markdown editor or press `Ctrl+Shift+V`
- **Version**: Uses latest Mermaid from CDN

### GitHub
- **Built-in Support**: All markdown files and GitHub Flavored Markdown (GFM)
- **Where**: README.md, wiki pages, pull requests, issues, discussions
- **Version**: Updates automatically with GitHub platform

### Other Platforms
- **GitLab**: Supported natively
- **Notion**: Supported via integrations
- **Confluence**: Supported via plugins
- **Docusaurus**: Native support

---

## Diagram Types

| Type | Use Case | Best For |
|------|----------|----------|
| **Flowchart** | Decision trees, workflows, processes | Feature flows, API flows |
| **Sequence** | Interactions over time | Multi-service calls, handshakes |
| **Class** | Object relationships | Data models, entity diagrams |
| **State** | State transitions | Lifecycle, FSM |
| **ER (Entity-Relationship)** | Database schemas | Data structures |
| **Gantt** | Timeline/schedules | Sprint planning, project schedules |
| **Git Graph** | Branch/commit history | Git workflow visualization |
| **Pie/Bar** | Data visualization | Metrics, distribution |
| **Mind Map** | Hierarchical ideas | Planning, brainstorming |
| **Timeline** | Historical events | Release history, milestones |

---

## Basic Syntax

### Structure
```mermaid
graph TD
    A[Start] --> B{Decision}
    B -->|Yes| C[Process 1]
    B -->|No| D[Process 2]
    C --> E[End]
    D --> E
```

### Node Types
```mermaid
graph TD
    A[Rectangle]
    B(Rounded)
    C{Diamond}
    D([Oval])
    E[/Parallelogram/]
    F[\Reversed\]
    G{{Hexagon}}
    H>Flag]
```

### Connections
```mermaid
graph TD
    A --> B           # Arrow
    A --- B           # Line
    A -->|Label| B    # Labeled arrow
    A -.->|Dotted| B  # Dotted arrow
    A ==>|Thick| B    # Thick arrow
```

### Styling
```mermaid
graph TD
    A[Node A]
    B[Node B]
    A --> B
    
    style A fill:#e1f5ff,stroke:#01579b,stroke-width:2px
    style B fill:#e8f5e9,stroke:#1b5e20,stroke-width:2px
```

---

## Common Diagrams

### 1. Flowchart (Wolverine HTTP Handler Lifecycle)

```mermaid
graph TD
    A["HTTP Request<br/>POST /command"] --> B["Wolverine<br/>Routing"]
    B --> C["Dependency<br/>Injection"]
    C --> D["Service.PublicAsyncMethod<br/>invoked"]
    D --> E["Validation<br/>FluentValidation"]
    E -->|Valid| F["Business Logic<br/>Execute"]
    E -->|Invalid| X["Return Error<br/>400"]
    F --> G["Tenant Filter<br/>Multi-tenant check"]
    G --> H["Database<br/>Query/Update"]
    H --> I["Publish Events<br/>Wolverine"]
    I --> J["Return Response<br/>200/201"]
    J --> K["Event Bus<br/>Async subscribers"]
    
    style A fill:#e1f5ff
    style D fill:#e8f5e9
    style E fill:#fff3e0
    style F fill:#e8f5e9
    style G fill:#fce4ec
    style H fill:#f3e5f5
    style J fill:#e1f5ff
```

### 2. Sequence Diagram (Multi-Service Communication)

```mermaid
sequenceDiagram
    Frontend->>StoreGateway: POST /checkout
    StoreGateway->>Identity: Verify token
    Identity-->>StoreGateway: Token valid
    StoreGateway->>Catalog: Get product prices
    Catalog->>DB: Query products
    DB-->>Catalog: Products
    Catalog-->>StoreGateway: Prices
    StoreGateway->>Orders: Create order
    Orders->>DB: Save order
    Orders-->>StoreGateway: Order created
    StoreGateway-->>Frontend: 201 Created
```

### 3. State Diagram (Order Lifecycle)

```mermaid
stateDiagram-v2
    [*] --> Pending: Order placed
    Pending --> Processing: Payment confirmed
    Processing --> Shipped: Items sent
    Shipped --> Delivered: Arrived
    Delivered --> [*]: Complete
    
    Pending --> Cancelled: Customer cancels
    Cancelled --> [*]: End
    
    Processing --> PaymentFailed: Payment fails
    PaymentFailed --> Pending: Retry
```

### 4. Class Diagram (Entity Relationships)

```mermaid
classDiagram
    class User {
        -Guid Id
        -Guid TenantId
        -string EmailEncrypted
        +CreateAsync()
        +UpdateAsync()
        +DeleteAsync()
    }
    
    class Order {
        -Guid Id
        -Guid UserId
        -decimal Amount
        +PlaceOrder()
        +CancelOrder()
    }
    
    User "1" --> "*" Order : places
```

### 5. ER Diagram (Database Schema)

```mermaid
erDiagram
    USERS ||--o{ ORDERS : places
    ORDERS ||--|{ ORDER_ITEMS : contains
    PRODUCTS ||--o{ ORDER_ITEMS : "has"
    USERS {
        guid id PK
        guid tenant_id FK
        string email_encrypted
    }
    ORDERS {
        guid id PK
        guid user_id FK
        decimal amount
        datetime created_at
    }
    PRODUCTS {
        guid id PK
        string sku
        decimal price
    }
```

### 6. Gantt Chart (Sprint Planning)

```mermaid
gantt
    title Sprint 2 Timeline
    dateFormat YYYY-MM-DD
    
    section Backend
    P0.1 Audit Logging :done, 2025-12-15, 2025-12-22
    P0.2 Encryption :active, 2025-12-22, 7d
    P0.3 Incident Response :crit, 2025-12-29, 10d
    
    section Frontend
    BITV Compliance :2025-12-15, 2025-12-31
    Component Library :2026-01-01, 14d
```

---

## Best Practices

### 1. Keep Diagrams Simple
```mermaid
graph TD
    A[Start] --> B[Process]
    B --> C[End]
```
✅ Clear and readable

```mermaid
graph TD
    A[Start] --> B{Decision 1}
    B -->|Yes| C{Decision 2}
    B -->|No| D{Decision 3}
    C -->|Maybe| E{Decision 4}
    E -->|Path 1| F{Decision 5}
    F -->|Yes| G[End]
    F -->|No| H[End]
    D -->|A| I[End]
    D -->|B| J[End]
```
❌ Too complex - break into multiple diagrams

### 2. Use Descriptive Labels
```mermaid
graph TD
    A["User submits form"]
    B["Validate input"]
    A --> B
```
✅ Clear action

```mermaid
graph TD
    A["Evt"]
    B["Chk"]
    A --> B
```
❌ Abbreviations confuse readers

### 3. Color Code by Priority/Category
```mermaid
graph TD
    A["Critical path"]:::critical
    B["Important"]:::important
    C["Optional"]:::optional
    
    classDef critical fill:#ffcdd2,stroke:#c62828
    classDef important fill:#fff3e0,stroke:#e65100
    classDef optional fill:#e8f5e9,stroke:#2e7d32
```

### 4. Use Subgraphs for Organization
```mermaid
graph TD
    subgraph Frontend["Frontend Layer"]
        A["Vue Component"]
        B["Pinia Store"]
    end
    
    subgraph Backend["Backend Layer"]
        C["HTTP Handler"]
        D["Database"]
    end
    
    A --> B
    B --> C
    C --> D
```

### 5. Document Complex Flows with Annotations
```mermaid
graph TD
    A["POST /order<br/>Create Order"] -->|Validate| B["Check stock"]
    B -->|In stock| C["Create transaction"]
    B -->|Out of stock| D["Return 400"]
    C -->|Success| E["Publish event"]
    C -->|Fail| F["Rollback"]
```

---

## Troubleshooting

### Issue: Diagram not rendering in VS Code
**Solution**:
1. Ensure markdown.mermaid.enabled = true in settings
2. Reload VS Code (Ctrl+R or Cmd+R)
3. Check browser console for errors (if using markdown preview)

### Issue: Diagram renders differently on GitHub
**Cause**: GitHub uses different CSS/rendering  
**Solution**: 
- Test locally first
- GitHub has better support for flowcharts/sequences than advanced diagrams
- Use simpler syntax for cross-platform compatibility

### Issue: Syntax Error: "No valid definition found"
**Cause**: Invalid syntax
**Solution**:
1. Check diagram type declaration (graph, sequenceDiagram, etc.)
2. Validate node connections (A --> B must have both A and B defined)
3. Use online editor: https://mermaid.live

### Issue: Node text overlapping
**Cause**: Long text in small diagram
**Solution**:
- Use line breaks: `["Line 1<br/>Line 2"]`
- Shorten labels
- Make diagram wider

### Issue: Arrows not appearing as expected
**Common mistakes**:
```mermaid
% ❌ Wrong - missing node definition
A --> B
% B not defined

% ✅ Correct
A[Node A] --> B[Node B]
```

---

## Syntax Reference

### Flowchart Directions
```
graph TD    % Top to Down
graph LR    % Left to Right
graph DU    % Down to Up
graph RL    % Right to Left
```

### Connection Types
```
-->         % Arrow
---         % Line
-.->        % Dotted arrow
==>         % Thick arrow
--text-->   % Labeled
```

### Node Shapes
```
[text]      % Rectangle
(text)      % Rounded
{text}      % Diamond
([text])    % Oval/Stadium
[/text/]    % Parallelogram
[\text\]    % Reversed parallelogram
{{text}}    % Hexagon
>text]      % Flag
[(text)]    % Subroutine
```

---

## Resources

- **Official Docs**: https://mermaid.js.org/
- **Live Editor**: https://mermaid.live/
- **GitHub Support**: https://github.blog/2022-02-14-include-diagrams-in-your-markdown-files-with-mermaid/
- **VS Code Docs**: https://code.visualstudio.com/docs/languages/markdown#_mermaid

---

## Examples in This Project

### Architecture Diagrams
- [Backend Handler Lifecycle](../../../.github/instructions/backend.instructions.md)
- [Frontend Component Lifecycle](../../../.github/instructions/frontend.instructions.md)
- [Security P0 Components](../../../.github/instructions/security.instructions.md)
- [Infrastructure Topology](../../../.github/instructions/devops.instructions.md)

### Usage Guidelines
- Use for architecture/design decisions
- Prefer Mermaid over ASCII art
- Include in: Architecture Decision Records, API documentation, process flows
- Keep in: `.github/instructions/` for reference, agent files for quick commands

---

## When to Use Each Diagram Type

| Goal | Diagram | Example |
|------|---------|---------|
| Show workflow | Flowchart | Feature development process |
| Show communication | Sequence | API calls between services |
| Show states | State | Order lifecycle |
| Show data model | ER Diagram | Database schema |
| Show object structure | Class | Entity relationships |
| Show timeline | Gantt | Sprint schedule |
| Show hierarchy | Mind Map | Project breakdown |

---

## Tips for Integration

1. **In Code Reviews**: Use Mermaid to explain complex changes
2. **In Architecture Docs**: Every significant architecture should have a diagram
3. **In PR Descriptions**: Add diagram to show what's changing
4. **In Agent Instructions**: Use diagrams to explain concepts
5. **In Runbooks**: Flowcharts for incident response procedures

