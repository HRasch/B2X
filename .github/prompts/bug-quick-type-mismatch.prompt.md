---
docid: PRM-037
title: Bug Quick Type Mismatch.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QBF-TYPE
title: Quick Bug Fix - TypeScript Type Mismatches
command: /bug-type-mismatch
owner: @TechLead
status: Active
references: ["ADR-052", "KB-053"]
---

# /bug-type-mismatch - Quick TypeScript Type Mismatch Fix

**Use when**: TypeScript type errors, incompatible types, type assertion issues

---

## Diagnostic MCP Chain

```bash
# 1. Get type error details
typescript-mcp/analyze_types filePath="[FILE_PATH]"

# 2. Find symbol usages and type sources
typescript-mcp/find_symbol_usages symbol="[TYPE_OR_VARIABLE]"

# 3. Validate fix
typescript-mcp/validate_types filePath="[FILE_PATH]"
```

---

## Execution Steps

### Step 1: Gather Context
Provide:
- **File**: Path to file with type error (e.g., `src/utils/parser.ts`)
- **Error**: TypeScript error message (e.g., `Type 'string | null' is not assignable to type 'string'`)
- **Line**: Line number (e.g., `25`)

### Step 2: Run MCP Analysis
1. Analyze types at error location
2. Inspect variable declarations
3. Check function signatures
4. Review type annotations
5. Find type mismatch source

### Step 3: Choose Fix Strategy

**Strategy 1: Type Guard (Recommended)**
```typescript
// ❌ Error: Type 'string | null' is not assignable to type 'string'
function process(value: string | null): string {
  return value.toUpperCase(); // Error!
}

// ✅ Fix: Type guard narrows the type
function process(value: string | null): string {
  if (value === null) throw new Error('Value is null');
  return value.toUpperCase(); // Now type is string
}
```

**Strategy 2: Optional Handling**
```typescript
// ❌ Error: Method expects string, got string | undefined
const name: string | undefined = getName();
display(name); // Error!

// ✅ Fix: Provide default or guard
const name: string | undefined = getName();
display(name ?? 'Unknown'); // Now always string
```

**Strategy 3: Type Predicate (Advanced)**
```typescript
// ❌ Error: Type 'unknown' is not assignable to type 'User'
function parseUser(data: unknown): User {
  return data as User; // Unsafe!
}

// ✅ Fix: Type predicate validates type
function isUser(data: unknown): data is User {
  return (
    typeof data === 'object' &&
    data !== null &&
    'id' in data &&
    'name' in data
  );
}

function parseUser(data: unknown): User {
  if (!isUser(data)) throw new Error('Invalid user data');
  return data; // Type is now User
}
```

**Strategy 4: Interface Extension**
```typescript
// ❌ Error: 'extraProperty' does not exist on type 'User'
interface User {
  id: number;
  name: string;
}

const user: User = {
  id: 1,
  name: 'John',
  extraProperty: true // Error!
};

// ✅ Fix: Extend or update interface
interface User {
  id: number;
  name: string;
  extraProperty?: boolean; // Add property
}
```

**Strategy 5: Generic Type Parameter**
```typescript
// ❌ Error: Argument of type string | number is not assignable
function getValue<T extends string>(input: T): T {
  return input;
}

// Usage error: string | number is not assignable to string
getValue<string | number>('test'); // Error!

// ✅ Fix: Constrain the generic or use union
function getValue<T extends string | number>(input: T): T {
  return input; // Now accepts both string and number
}
```

### Step 4: Validate Fix
```bash
# Run type checker
typescript-mcp/validate_types filePath="[FILE_PATH]"

# No TypeScript errors should remain
# Type safety is restored
```

### Step 5: Document
Add to `.ai/knowledgebase/lessons.md`:
```markdown
## [Quick Fix] TypeScript Type Safety

**Pattern**: Use type guards and optional chaining, avoid `as` casts
**Files**: [list of files]
**Prevention**: 
  - Enable strict mode in tsconfig.json
  - Use type predicates for runtime checks
  - Avoid `as Type` assertions (use type guards instead)
  - Update interface definitions, don't ignore errors
**Tool**: typescript-mcp
```

---

## Common Patterns

### Pattern 1: Union Type Narrowing
```typescript
// ❌ Error: Can't call method on string | number
function double(value: string | number): number {
  return value * 2; // Error! string can't multiply
}

// ✅ Fix: Type guard narrows union
function double(value: string | number): number {
  if (typeof value === 'string') {
    return parseInt(value) * 2;
  }
  return value * 2;
}
```

### Pattern 2: Optional Property
```typescript
// ❌ Error: Property might not exist
const user: { id: number; age?: number } = { id: 1 };
const age: number = user.age; // Error! Could be undefined

// ✅ Fix: Provide default or guard
const age: number = user.age ?? 0;
```

### Pattern 3: Array Element Type
```typescript
// ❌ Error: Array element might not be what you expect
const items: (string | number)[] = [1, 'two', 3];
const firstItem: string = items[0]; // Error! Could be number

// ✅ Fix: Type guard or cast properly
const firstItem: string | number = items[0];
// or
if (typeof items[0] === 'string') {
  const firstItem: string = items[0];
}
```

### Pattern 4: Function Return Type
```typescript
// ❌ Error: Function might return undefined
function getName(): string {
  if (someCondition) {
    return 'John';
  }
  // Implicitly returns undefined - Error!
}

// ✅ Fix: Return proper type or update signature
function getName(): string | null {
  if (someCondition) {
    return 'John';
  }
  return null; // Explicit return
}
```

---

## Type Guard Patterns (Quick Reference)

```typescript
// String guard
if (typeof value === 'string') { }

// Number guard
if (typeof value === 'number') { }

// Object guard
if (value !== null && typeof value === 'object') { }

// Array guard
if (Array.isArray(value)) { }

// Class instance guard
if (value instanceof MyClass) { }

// Null/undefined guard
if (value != null) { } // Checks both null and undefined
if (value !== null && value !== undefined) { }

// Property guard
if ('propertyName' in value) { }

// Type predicate (custom)
function isUser(value: unknown): value is User {
  return value instanceof User; // or custom logic
}
```

---

## Quick Checklist

- [ ] Identified the type mismatch
- [ ] Chose appropriate fix strategy (guard, optional, predicate)
- [ ] Type guard (if used) properly narrows type
- [ ] Avoided unsafe `as` type assertions
- [ ] Updated interface if needed
- [ ] TypeScript type check passes
- [ ] No `// @ts-ignore` or `// @ts-expect-error` without justification
- [ ] Tests cover type paths
- [ ] Lesson documented in lessons.md

---

## Anti-Patterns to Avoid

❌ **Don't use `any` type**:
```typescript
const value: any = getData(); // Loses type safety!
```

❌ **Don't use unsafe type assertions**:
```typescript
const user = data as User; // Could fail at runtime!
```

❌ **Don't suppress type errors without reason**:
```typescript
// @ts-ignore - WRONG! Hides real issues
const value: string = maybeNull;
```

---

## See Also

- [ADR-052] MCP-Enhanced Bugfixing Workflow
- [KB-053] TypeScript MCP Integration
- TypeScript Handbook: https://www.typescriptlang.org/docs/handbook/
- Type Guards: https://www.typescriptlang.org/docs/handbook/2/narrowing.html
- Strict Mode: https://www.typescriptlang.org/tsconfig#strict
