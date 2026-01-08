---
docid: PRM-032
title: Bug Quick Async Race.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QBF-ASYNC
title: Quick Bug Fix - Async Race Conditions
command: /bug-async-race
owner: @TechLead
status: Active
references: ["ADR-052", "KB-053"]
---

# /bug-async-race - Quick Async Race Condition Fix

**Use when**: Data undefined after async operation, race conditions, missing awaits, stale state

---

## Diagnostic MCP Chain

```bash
# 1. Type analysis to find async functions
typescript-mcp/analyze_types filePath="[FILE_PATH]"

# 2. Find async function usages
typescript-mcp/find_symbol_usages symbol="[FUNCTION_NAME]"

# 3. Detect missing awaits via type analysis
typescript-mcp/validate_types filePath="[FILE_PATH]"
```

---

## Execution Steps

### Step 1: Gather Context
Provide:
- **File**: Path to file with race condition (e.g., `src/components/SearchResults.vue`)
- **Function**: Async function name (e.g., `fetchResults`)
- **Symptom**: What goes wrong (e.g., "data is sometimes undefined")

### Step 2: Run MCP Analysis
1. Analyze async function signature
2. Check where it's called
3. Identify missing `await` keywords
4. Check for simultaneous requests (race condition)
5. Verify state guards exist

### Step 3: Generate Fix

**Pattern 1: Missing Await**
```typescript
// ❌ Race condition - not waiting for promise
fetchData();
useData(data); // data still undefined!

// ✅ Fix: Await the promise
const data = await fetchData();
useData(data);
```

**Pattern 2: Vue Async Handling**
```vue
<!-- ❌ Race condition - no loading state -->
<div>{{ results }}</div>

<!-- ✅ Fix: Check loading state first -->
<div v-if="loading">Loading...</div>
<div v-else-if="results">{{ results }}</div>
```

**Pattern 3: Concurrent Requests**
```typescript
// ❌ Race condition - simultaneous requests
search();
search(); // Second overwrites first!

// ✅ Fix: Cancel previous request
const abortController = new AbortController();

function search() {
  abortController.abort(); // Cancel previous
  const newController = new AbortController();
  abortController = newController;
  
  fetch(url, { signal: newController.signal })
    .then(handleResults);
}
```

**Pattern 4: Promise.all for Parallel Waits**
```typescript
// ❌ Sequential waits - slow
const user = await getUser();
const orders = await getOrders(user.id);

// ✅ Fix: Parallel when independent
const [user, globalData] = await Promise.all([
  getUser(),
  getGlobalData()
]);
```

### Step 4: Add State Guards

Always pair async with loading state:

```vue
<script setup lang="ts">
import { ref } from 'vue';

const data = ref<Data | null>(null);
const loading = ref(false);
const error = ref<string | null>(null);

async function fetchData() {
  loading.value = true;
  error.value = null;
  try {
    data.value = await api.getData();
  } catch (e) {
    error.value = e.message;
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <!-- ✅ Guards prevent rendering undefined data -->
  <div v-if="loading">Loading...</div>
  <div v-else-if="error">{{ error }}</div>
  <div v-else-if="data">{{ data.content }}</div>
</template>
```

### Step 5: Validate
- ✅ All async calls use `await`
- ✅ State guards exist (loading, error)
- ✅ Type check passes
- ✅ No race condition with AbortController
- ✅ Tests verify async handling

### Step 6: Document
Add to `.ai/knowledgebase/lessons.md`:
```markdown
## [Quick Fix] Async Race Prevention

**Pattern**: Always await async operations; use loading states
**Files**: [list of files]
**Prevention**: 
  - Add `await` to all async calls
  - Implement loading/error states
  - Use AbortController for cancellable operations
  - Test async behavior explicitly
**Tool**: typescript-mcp
```

---

## Common Patterns

### Pattern 1: Forgot Await
```typescript
// ❌ Bug: Promise returned, not awaited
const task = () => {
  const data = getUserData(); // Returns Promise<User>
  return data.name; // Error: Promise has no .name property
};

// ✅ Fix: Await the promise
const task = async () => {
  const data = await getUserData();
  return data.name;
};
```

### Pattern 2: Race Condition in Watchers
```typescript
// ❌ Bug: Multiple requests can race
watch(() => query.value, async (newQuery) => {
  results.value = await search(newQuery);
});

// ✅ Fix: Cancel previous request
const abortController = ref<AbortController | null>(null);

watch(() => query.value, async (newQuery) => {
  abortController.value?.abort();
  abortController.value = new AbortController();
  
  try {
    results.value = await search(newQuery, { signal: abortController.value.signal });
  } catch (e) {
    if (e.name !== 'AbortError') throw e;
  }
});
```

### Pattern 3: Parallel Data Fetching
```typescript
// ❌ Slow: Sequential fetches
const user = await fetchUser(id);
const orders = await fetchOrders(user.id);
const settings = await fetchSettings(user.id);

// ✅ Fast: Parallel fetches
const [user, orders, settings] = await Promise.all([
  fetchUser(id),
  fetchOrders(id),
  fetchSettings(id)
]);
```

---

## Quick Checklist

- [ ] Identified async function(s) involved
- [ ] All async calls have `await`
- [ ] Loading state exists
- [ ] Error handling exists (try-catch or error state)
- [ ] Render guards check loading/error state
- [ ] AbortController used for cancellable operations
- [ ] Type check passes
- [ ] Tests verify async behavior
- [ ] Lesson documented in lessons.md

---

## See Also

- [ADR-052] MCP-Enhanced Bugfixing Workflow
- [KB-053] TypeScript MCP Integration
- JavaScript async/await: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/async_function
- AbortController: https://developer.mozilla.org/en-US/docs/Web/API/AbortController
- Promise.all: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/all
