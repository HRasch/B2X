Title: C# 14 — concise summary
Source: https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14

Overview:
- C# 14 is the language version shipped with .NET 10. It focuses on developer productivity, safer patterns, and first-class support for spans/stack-only types.

Key features:
- Extension members: new `extension` blocks allow extension properties, extension methods, and static extension members (including static operators) attached to a target type.
- `field` keyword for field-backed property accessors: write accessor bodies that reference the compiler-generated backing field via `field`.
- First-class Span support: new implicit conversions and language support for `Span<T>` and `ReadOnlySpan<T>` to make span usage more ergonomic.
- `nameof` with unbound generics: `nameof(List<>)` is allowed and returns `"List"`.
- Lambda parameter modifiers: allow `ref`, `in`, `out`, `ref readonly`, `scoped` (etc.) on simple lambda parameters without explicit types.
- Partial constructors & partial events: extend partial members to constructors and events.
- Null-conditional assignment: allow `?.` and `?[]` on the left side of assignments and compound assignments (with limitations).
- User-defined compound assignment and custom ++/-- operators.

Notes & actionables:
- Enable the language version in project files when needed: `<LangVersion>preview</LangVersion>` or target the appropriate SDK where C# 14 is the default (e.g., .NET 10 SDK).
- Review compiler breaking changes for C# 14 at: https://learn.microsoft.com/dotnet/csharp/whats-new/breaking-changes
- Test and lint code that uses `Span<T>` conversions and new extension member patterns; ensure no unintended binary/behavioral changes.

References:
- C# 14 whats-new: https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14
- Feature proposals/specs: https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-14.0/
