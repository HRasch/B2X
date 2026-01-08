---
docid: UNKNOWN-172
title: Bug Async Race.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: TechLead
description: Quick fix for async race condition bugs
---

# Bug Quick Fix: Async Race Condition

Fix race conditions in asynchronous operations.

## Required Information
- File Path: 
- Component/Function: 
- Race Condition Description: 
- Affected State/Data: 

## Quick Fix Patterns

### Vue Composition API - Loading States
```vue
<script setup>
const isLoading = ref(false)
const data = ref(null)

const fetchData = async () => {
  isLoading.value = true
  try {
    const result = await apiCall()
    // Only update if still loading (prevents race)
    if (isLoading.value) {
      data.value = result
    }
  } finally {
    isLoading.value = false
  }
}

// In template
<div v-if="isLoading">Loading...</div>
<div v-else-if="data">{{ data }}</div>
</script>
```

### AbortController Pattern
```typescript
let abortController = null

const fetchData = async () => {
  // Cancel previous request
  if (abortController) {
    abortController.abort()
  }
  
  abortController = new AbortController()
  
  try {
    const result = await fetch('/api/data', {
      signal: abortController.signal
    })
    data.value = await result.json()
  } catch (error) {
    if (error.name !== 'AbortError') {
      console.error(error)
    }
  }
}
```

### State Guards
```typescript
// BEFORE - Race condition prone
const updateData = async (newData) => {
  await api.update(newData)
  localData.value = newData
}

// AFTER - With guard
const updateData = async (newData) => {
  const currentVersion = dataVersion.value
  await api.update(newData)
  // Check if state changed during async operation
  if (dataVersion.value === currentVersion) {
    localData.value = newData
  }
}
```

## Output Format

```
✅ Race Condition Fixed

Modified: [file path]
- Added [loading state / abort controller / state guard]
- Prevents [describe the race condition]
- Uses [pattern name] approach
```

## Testing
- [ ] Test rapid successive calls
- [ ] Verify state consistency
- [ ] Check error handling