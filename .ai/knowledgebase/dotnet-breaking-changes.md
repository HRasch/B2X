Title: .NET compatibility & breaking-changes — concise guidance
Source: https://learn.microsoft.com/dotnet/core/compatibility/

Overview:
- .NET maintains strong forward/backward compatibility guarantees but documents rules for what types of changes are allowed, disallowed, or require judgement. This document summarizes the key rules and recommended practices for library and runtime authors.

Key rules (summary):
- Public contract changes: renaming or removing public types/members is disallowed. Changing member/parameter/return types is disallowed. Adding members to interfaces or changing member signatures can be breaking and often requires careful judgement.
- Types & members: changing visibility, adding instance fields to structs, changing ref/ref readonly signatures on virtual/interface members, or adding overloads that change overload resolution can be breaking.
- Exceptions and behavior: introducing new exceptions on existing code paths is disallowed unless they are subclasses of existing exceptions; changing exception types that callers rely on can break consumers.
- Assemblies & platform support: renaming assemblies or changing public keys is disallowed; changing supported platforms should be clearly documented.
- Internal vs public changes: internal API surface changes are usually allowed but can break third-party consumers that rely on private reflection — exercise caution.
- Performance changes: allowed but can break code relying on timing or synchronous behavior; changing sync→async (or vice versa) is disallowed.

Practical actions for maintainers:
- Use the official compatibility checklist before publishing breaking changes: review [library change rules](https://learn.microsoft.com/dotnet/core/compatibility/library-change-rules).
- Add explicit release notes and compatibility guidance for each change.
- Prefer additive changes and provide migration paths (obsolete attributes, compatibility switches, runtime feature flags) when unavoidable.
- Run API compatibility analyzers and test suites across target TFMs.

References:
- Compatibility landing: https://learn.microsoft.com/dotnet/core/compatibility/
- Library change rules: https://learn.microsoft.com/dotnet/core/compatibility/library-change-rules
- Breaking changes index: https://learn.microsoft.com/dotnet/core/compatibility/breaking-changes
