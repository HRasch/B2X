---
agent: TechLead
description: Quick fix for TypeScript type mismatch errors
---

# Bug Quick Fix: Type Mismatch

Resolve TypeScript type errors and mismatches.

## Required Information
- File Path: 
- Line Number: 
- Error Message: 
- Expected Type: 
- Actual Type: 

## Quick Fix Patterns

### Type Assertion (when confident)
```typescript
// BEFORE - Type error
const data = JSON.parse(response) as MyType

// AFTER - Safe type assertion
const data = JSON.parse(response) as MyType
// Or better: proper typing
interface ApiResponse {
  data: MyType
}
const response: ApiResponse = await api.get()
const data = response.data
```

### Union Types for Flexibility
```typescript
// BEFORE - Strict type causing errors
const status: 'loading' | 'success' | 'error' = 'loading'

// AFTER - More flexible
const status: 'loading' | 'success' | 'error' | string = 'loading'
```

### Optional Properties
```typescript
// BEFORE - Required property causing issues
interface User {
  name: string
  email: string
}

// AFTER - Optional when data might be missing
interface User {
  name: string
  email?: string
}
```

### Generic Constraints
```typescript
// BEFORE - Unconstrained generic
function processData<T>(data: T) {
  return data.property // Error: property doesn't exist on T
}

// AFTER - Constrained generic
function processData<T extends { property: string }>(data: T) {
  return data.property
}
```

## Output Format

```
✅ Type Mismatch Resolved

Modified: [file path]
- Fixed type error: [error description]
- Applied [type assertion / union type / optional property / generic constraint]
- Maintains type safety while resolving error
```

## Testing
- [ ] TypeScript compilation passes
- [ ] No runtime type errors
- [ ] Maintains type safety