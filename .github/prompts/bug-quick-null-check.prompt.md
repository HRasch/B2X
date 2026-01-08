---
docid: PRM-035
title: Bug Quick Null Check.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QBF-NULL
title: Quick Bug Fix - Null/Undefined Reference Errors
command: /bug-null-check
owner: @TechLead
status: Active
references: ["ADR-052", "KB-053"]
---

# /bug-null-check - Quick Null Reference Fix

**Use when**: "Cannot read property X of undefined", null reference errors, missing optional chaining

---

## Diagnostic MCP Chain

```bash
# 1. Type analysis at error location
typescript-mcp/analyze_types filePath="[FILE_PATH]"

# 2. Symbol usage tracing
typescript-mcp/find_symbol_usages symbol="[VARIABLE_NAME]"

# 3. Validation
typescript-mcp/validate_types filePath="[FILE_PATH]"
```

---

## Execution Steps

### Step 1: Gather Context
Provide:
- **File**: Path to file with error (e.g., `src/components/ProductCard.vue`)
- **Variable**: Variable causing null error (e.g., `product.images`)
- **Line**: Line number of error (e.g., `45`)

### Step 2: Run MCP Analysis
1. Analyze types at location
2. Check where variable is assigned
3. Identify if assignment is conditional
4. Check if null/undefined checks exist

### Step 3: Generate Fix
Apply one or more:

**Vue Templates**:
```vue
<!-- Before -->
<img :src="product.images[0].url" />

<!-- After -->
<img v-if="product?.images?.[0]?.url" :src="product.images[0].url" />
```

**TypeScript/JavaScript**:
```typescript
// Before
const url = product.images[0].url;

// After
const url = product?.images?.[0]?.url ?? 'default.jpg';
```

**Type Guards**:
```typescript
// Before
function processData(data: { value: string }) {
  console.log(data.value);
}

// After
function processData(data: { value: string } | null) {
  if (!data) return;
  console.log(data.value);
}
```

### Step 4: Validate
- ✅ Type check passes
- ✅ No new TypeScript errors
- ✅ Component renders without errors
- ✅ Update related tests if needed

### Step 5: Document
Add to `.ai/knowledgebase/lessons.md`:
```markdown
## [Quick Fix] Null Reference Prevention

**Pattern**: Always use optional chaining (?.) when accessing nested properties
**Files**: [list of files]
**Prevention**: Check for undefined before template rendering with v-if
**Tool**: typescript-mcp
```

---

## Common Patterns

### Pattern 1: Array Access
```typescript
// ❌ Error: items might be undefined
items[0].name

// ✅ Fix: Optional chaining
items?.[0]?.name
```

### Pattern 2: Nested Objects
```typescript
// ❌ Error: user might not have settings
user.settings.theme

// ✅ Fix: Chain the optionals
user?.settings?.theme
```

### Pattern 3: Function Returns
```typescript
// ❌ Error: function might return null
const result = getData();
process(result.value);

// ✅ Fix: Guard the result
const result = getData();
if (result !== null) {
  process(result.value);
}
```

### Pattern 4: Vue Template
```vue
<!-- ❌ Error: product could be undefined -->
<h1>{{ product.name }}</h1>

<!-- ✅ Fix: Use v-if guard -->
<h1 v-if="product">{{ product.name }}</h1>
```

---

## Quick Checklist

- [ ] Identified the problematic variable
- [ ] Checked where it's assigned
- [ ] Added null/undefined checks or optional chaining
- [ ] Type validation passes
- [ ] Tests updated
- [ ] Lesson documented in lessons.md

---

## See Also

- [ADR-052] MCP-Enhanced Bugfixing Workflow
- [KB-053] TypeScript MCP Integration
- TypeScript Optional Chaining: https://www.typescriptlang.org/docs/handbook/release-notes/typescript-3-7.html#optional-chaining
