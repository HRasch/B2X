---
docid: UNKNOWN-175
title: Bug Null Check.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: TechLead
description: Quick fix for null reference bugs
---

# Bug Quick Fix: Null Reference

Apply defensive null checks to prevent null reference errors.

## Required Information
- File Path: 
- Line Number: 
- Variable/Expression: 
- Error Message: 

## Quick Fix Pattern

### For Vue Templates
```vue
<!-- BEFORE -->
<div>{{ user.name }}</div>

<!-- AFTER -->
<div>{{ user?.name }}</div>
```

### For JavaScript/TypeScript
```typescript
// BEFORE
const result = data.property.method();

// AFTER
const result = data?.property?.method();
```

### For Arrays/Objects
```typescript
// BEFORE
items.forEach(item => console.log(item.value));

// AFTER
items?.forEach(item => console.log(item.value));
```

## Output Format

```
✅ Null Check Applied

Modified: [file path]
- Added optional chaining to [variable/expression]
- Prevents null reference error in [context]
```

## Testing
- [ ] Verify no runtime errors
- [ ] Check edge cases (empty arrays, null objects)
- [ ] Run existing tests