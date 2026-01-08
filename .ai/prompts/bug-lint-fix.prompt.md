---
agent: TechLead
description: Quick fix for linting errors
---

# Bug Quick Fix: Linting Errors

Auto-fix common linting issues.

## Required Information
- File Path: 
- Linter: [ESLint / StyleCop / Prettier]
- Error Message: 
- Rule Violated: 

## Quick Fix Patterns

### ESLint - Vue
```vue
<!-- BEFORE - Missing key -->
<div v-for="item in items">
  {{ item.name }}
</div>

<!-- AFTER - Add key -->
<div v-for="item in items" :key="item.id">
  {{ item.name }}
</div>
```

```javascript
// BEFORE - Unused variable
const unusedVar = 'test'
console.log('hello')

// AFTER - Remove or prefix with underscore
const _unusedVar = 'test' // or remove entirely
console.log('hello')
```

### Prettier - Formatting
```typescript
// BEFORE - Poor formatting
const data=await api.get();if(data){process(data)}

// AFTER - Proper formatting
const data = await api.get()
if (data) {
  process(data)
}
```

### StyleCop - C#
```csharp
// BEFORE - Missing braces
if (condition)
    return true;

// AFTER - Add braces
if (condition)
{
    return true;
}
```

## Output Format

```
✅ Linting Error Fixed

Modified: [file path]
- Fixed [linter] rule: [rule name]
- Applied [auto-fix / manual correction]
- Code now passes linting
```

## Testing
- [ ] Linter passes without errors
- [ ] Code functionality unchanged
- [ ] Formatting consistent