---
docid: KB-132
title: Backend Index
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Backend Lessons Index

**DocID**: KB-LESSONS-BACKEND-INDEX
**Category**: Backend | **Priority Focus**: .NET, C#, Wolverine, PostgreSQL, APIs
**Last Updated**: 8. Januar 2026 | **Owner**: @DocMaintainer

---

## � Search & Tags

**Technology Tags**: `dotnet` `csharp` `wolverine` `postgresql` `cqrs` `validation` `architecture` `performance`
**Problem Tags**: `monolithic` `duplication` `complexity` `performance` `architecture` `validation` `database`
**Solution Tags**: `refactor` `extract` `centralize` `optimize` `cqrs` `indexing` `patterns`
**Impact Tags**: `maintainability` `performance` `scalability` `code-quality` `architecture`

**Quick Search**:
- Architecture issues: [🔴 Monolithic File Complexity](#-critical-must-know---architecture--performance), [🔴 Validation Pattern Duplication](#-critical-must-know---architecture--performance)
- Performance problems: [🟡 CQRS Implementation](#-important-should-know---implementation-patterns), [🟡 Database Optimization](#-important-should-know---implementation-patterns)
- Code quality: [🔴 Monolithic File Complexity](#-critical-must-know---architecture--performance)

---

## �🔴 Critical (Must Know - Architecture & Performance)

1. **Monolithic File Complexity** - Systematic extraction strategy
   - **Issue**: McpTools.cs grew to 1429 LOC with 11+ classes
   - **Root Cause**: Organic growth without file organization
   - **Solution**: Extract classes to focused files (80% size reduction)
   - **Reference**: [KB-LESSONS-BACKEND-RED-MONOLITHIC]
   - **Related**: [ADR-002] Onion Architecture, [KB-011] Patterns & Antipatterns

2. **Validation Pattern Duplication** - Centralized validation framework
   - **Issue**: 12+ duplicate validation patterns (~140 LOC duplication)
   - **Root Cause**: Copy-paste development without shared infrastructure
   - **Solution**: Created ValidatedBase<TRequest> with consistent patterns
   - **Reference**: [KB-LESSONS-BACKEND-RED-VALIDATION]
   - **Related**: [KB-001] C# 14 Features, [KB-011] Patterns & Antipatterns

---

## 🟡 Important (Should Know - Implementation Patterns)

3. **CQRS Implementation** - Command Query Responsibility Segregation
   - **Issue**: Mixed read/write operations in single handlers
   - **Root Cause**: Lack of clear separation of concerns
   - **Solution**: Separate command and query handlers
   - **Reference**: [KB-LESSONS-BACKEND-YELLOW-CQRS]
   - **Related**: [ADR-001] Wolverine over MediatR, [KB-006] Wolverine Patterns

4. **Database Optimization** - Query performance and indexing
   - **Issue**: Slow database queries impacting performance
   - **Root Cause**: Missing indexes, inefficient queries
   - **Solution**: Strategic indexing and query optimization
   - **Reference**: [KB-LESSONS-BACKEND-YELLOW-DATABASE]
   - **Related**: [KB-005] .NET Localization, [KB-011] Patterns & Antipatterns

---


## 🟢 Recent Additions (Last 30 Days)

*Recent lessons added to the knowledge base*

---

*Full details in archive files. This index prioritizes prevention.*

