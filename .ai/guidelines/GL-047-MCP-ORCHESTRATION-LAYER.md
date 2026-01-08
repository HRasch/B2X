---
docid: GL-083
title: GL 047 MCP ORCHESTRATION LAYER
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-047
title: MCP-Orchestration Layer - Intelligent Context Routing
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# GL-047: MCP-Orchestration Layer - Intelligent Context Routing

**Estimate**: 30-40% Token Savings | **Effort**: ⭐⭐ Medium

## Purpose

Automatically select the **optimal combination of GL-043/044/045 strategies** based on task context, eliminating manual decision-making and maximizing token efficiency.

---

## 🎯 Core Concept

Instead of manually choosing which optimization to apply, the **MCP-Orchestration Layer** intelligently routes work through the right combination:

```
TASK INPUT
    ↓
┌─────────────────────────────────┐
│ Context Analysis:               │
│ - File path                     │
│ - Task type                     │
│ - Knowledge requirements        │
│ - Code complexity               │
└─────────────────────────────────┘
    ↓
┌─────────────────────────────────┐
│ Decision Engine:                │
│ Apply GL-043? Apply GL-044?     │
│ Apply GL-045? All?              │
└─────────────────────────────────┘
    ↓
OPTIMIZED CONTEXT
(Minimal token overhead!)
```

---

## 🔍 Decision Engine Rules

### Rule 1: File Path Analysis (GL-043 Selection)

```
IF file_path matches:
  - src/components/** → Load frontend-essentials.md ONLY
  - src/api/** → Load backend-essentials.md ONLY
  - **/*.test.* → Load testing.md ONLY
  - .github/** → Load devops.md ONLY
  - (ALL) → Load security.md ALWAYS

ELSE:
  - Load minimal instruction set
```

**Token Savings**: 50-70% (GL-043)

**Example**:
```
File: src/components/ProductCard.vue
Decision: Load frontend-essentials.md + security.md ONLY
Saves: 6,000 tokens (skip backend, devops, testing)
```

---

### Rule 2: Task Type Analysis (GL-044 vs GL-045 Selection)

```
IF task == "understand_codebase":
  USE: semantic_search(concept)
  → Direct answer, no file reads needed
  → Cost: 1,500 tokens

ELSE IF task == "find_implementation":
  USE: grep_search(pattern) → read_file(targeted)
  → Locate first, then read only that section
  → Cost: 2,000-3,000 tokens (vs 8,000+ full file)

ELSE IF task == "learn_pattern":
  USE: kb-mcp/search_knowledge_base
  → Query KB for pattern examples
  → Cost: 1,500 tokens (vs 5,000+ pre-loaded articles)

ELSE IF task == "analyze_large_file":
  USE: semantic_search(structure)
  → Pattern-based analysis without full read
  → Cost: 1,500 tokens (vs 8,000+ full read)
```

**Token Savings**: 40-60% (GL-044 + GL-045)

---

### Rule 3: Knowledge Requirements (GL-045 Selection)

```
IF knowledge_needed == None:
  SKIP: KB articles entirely
  → Cost: 0 tokens

ELSE IF knowledge_needed == "specific_pattern":
  USE: kb-mcp/search_knowledge_base query:[pattern]
  → Targeted query, not full article
  → Cost: 1,500 tokens

ELSE IF knowledge_needed == "overview":
  USE: kb-mcp/search_knowledge_base with max_results:5
  → Get summary + key points
  → Cost: 1,500 tokens

ELSE IF knowledge_needed == "comprehensive":
  USE: kb-mcp/get_article docid:[KB-XXX]
  → Full article if truly needed
  → Cost: 2,000 tokens (acceptable for complex work)

NEVER: Pre-load articles "just in case"
```

**Token Savings**: 60% (GL-045)

---

## 📋 Orchestration Decision Trees

### Scenario 1: "Fix bug in backend service"

```
Task: Fix bug in ProductService.cs

Step 1: File Path Analysis (GL-043)
  Path: backend/Domain/Catalog/Services/ProductService.cs
  Decision: Load backend-essentials.md + security.md
  Cost: 3,200 tokens

Step 2: Task Type Analysis (GL-044)
  Type: "find_implementation"
  Action: grep_search("ProductQuery") → read_file(targeted)
  Cost: 2,500 tokens

Step 3: Knowledge Analysis (GL-045)
  Knowledge Needed: "Pattern understanding" (Wolverine handlers)
  Action: Query kb-mcp/search_knowledge_base
  Cost: 1,500 tokens

Total Overhead: 7,200 tokens
vs Old Way: 16,000 tokens
Savings: 55% ✅
```

---

### Scenario 2: "Build new Vue component"

```
Task: Create new ProductCard component

Step 1: File Path Analysis (GL-043)
  Path: src/components/ProductCard.vue
  Decision: Load frontend-essentials.md + security.md
  Cost: 3,100 tokens

Step 2: Task Type Analysis (GL-044)
  Type: "learn_pattern"
  Action: semantic_search("Vue responsive component patterns")
  Cost: 1,500 tokens
  (No file reads needed!)

Step 3: Knowledge Analysis (GL-045)
  Knowledge Needed: "i18n with Vue 3"
  Action: Query kb-mcp/search_knowledge_base
  Cost: 1,500 tokens

Total Overhead: 6,100 tokens
vs Old Way: 14,000 tokens
Savings: 56% ✅
```

---

### Scenario 3: "Review complex architecture"

```
Task: Review multi-tenant catalog integration

Step 1: File Path Analysis (GL-043)
  Path: Multiple (backend + architecture)
  Decision: Load backend-essentials.md + security.md
  Cost: 3,200 tokens
  (DevOps/Testing NOT needed)

Step 2: Task Type Analysis (GL-044)
  Type: "analyze_large_file"
  Action: semantic_search("multi-tenant domain isolation pattern")
  Cost: 1,500 tokens

Step 3: Knowledge Analysis (GL-045)
  Knowledge Needed: "Architecture patterns"
  Action: Query kb-mcp/search_knowledge_base
         category: "architecture"
  Cost: 1,500 tokens

Total Overhead: 6,200 tokens
vs Old Way: 20,000 tokens (multiple files + articles)
Savings: 69% ✅
```

---

## 🤖 Automated Implementation Pseudo-Code

```python
class ContextOrchestrator:
    """Intelligently routes work through optimal GL-043/044/045 combination"""
    
    def __init__(self, task, file_path):
        self.task = task
        self.file_path = file_path
        self.context = {}
        
    def analyze_context(self):
        """Determine what optimizations to apply"""
        
        # GL-043: Path-specific instructions
        self.context['instructions'] = self._select_instructions()
        
        # GL-044: File access strategy
        self.context['file_access'] = self._select_file_access()
        
        # GL-045: Knowledge loading strategy
        self.context['knowledge'] = self._select_knowledge()
        
        return self.context
    
    def _select_instructions(self):
        """Apply GL-043: Select path-specific instructions only"""
        instructions = []
        
        # Always add security
        instructions.append('security.instructions.md')
        
        # Add path-specific
        if 'components/' in self.file_path or 'frontend/' in self.file_path:
            instructions.append('frontend-essentials.md')
        elif 'api/' in self.file_path or 'backend/' in self.file_path:
            instructions.append('backend-essentials.md')
        elif 'test' in self.file_path:
            instructions.append('testing.md')
        elif 'github/' in self.file_path or 'Dockerfile' in self.file_path:
            instructions.append('devops.md')
        
        return {
            'attach': instructions,
            'total_size_kb': sum(estimate_size(i) for i in instructions),
            'strategy': 'GL-043 (path-specific only)'
        }
    
    def _select_file_access(self):
        """Apply GL-044: Select optimal file access pattern"""
        
        if self.task == 'understand_codebase':
            return {
                'method': 'semantic_search',
                'cost_tokens': 1500,
                'strategy': 'GL-044 (pattern-based, no file reads)'
            }
        
        elif self.task == 'find_implementation':
            return {
                'method': 'grep_search → read_file(targeted)',
                'cost_tokens': 2500,
                'strategy': 'GL-044 (locate then read fragments)'
            }
        
        elif self.task == 'analyze_structure':
            return {
                'method': 'grep_search(patterns)',
                'cost_tokens': 1000,
                'strategy': 'GL-044 (pattern analysis only)'
            }
        
        else:  # Default: fragment-based
            return {
                'method': 'grep_search → read_file(targeted)',
                'cost_tokens': 2500,
                'strategy': 'GL-044 (conservative: grep first)'
            }
    
    def _select_knowledge(self):
        """Apply GL-045: Query KB on-demand, never pre-load"""
        
        if not self.task.requires_external_knowledge:
            return {
                'kb_needed': False,
                'cost_tokens': 0,
                'strategy': 'GL-045 (no KB needed)'
            }
        
        else:
            return {
                'method': 'kb-mcp/search_knowledge_base',
                'cost_tokens': 1500,
                'strategy': 'GL-045 (query on-demand)'
            }
    
    def execute(self):
        """Run orchestrated context setup"""
        context = self.analyze_context()
        
        # Load instructions (GL-043)
        for instruction in context['instructions']['attach']:
            attach(instruction)
        
        # Prepare file access method (GL-044)
        set_file_access_strategy(context['file_access']['method'])
        
        # Enable KB querying (GL-045)
        if context['knowledge']['kb_needed']:
            enable_kb_query()
        
        # Report efficiency
        total_tokens = (
            context['instructions']['total_size_kb'] * 330 +  # ~330 tokens per KB
            context['file_access']['cost_tokens'] +
            context['knowledge']['cost_tokens']
        )
        
        return {
            'context': context,
            'total_overhead_tokens': total_tokens,
            'estimated_savings_percent': self._calculate_savings()
        }
```

---

## 📊 Orchestration Examples

### Example 1: Backend Bug Fix

```yaml
Input:
  task: "fix_bug"
  file: "backend/Domain/Catalog/Application/Handlers/CreateProductHandler.cs"
  issue: "SQL timeout on bulk operations"

Orchestration Analysis:
  1. GL-043: backend-essentials.md + security.md (3,200 tokens)
  2. GL-044: grep("CreateProduct") → read_file(42-95) (2,000 tokens)
  3. GL-045: query("Wolverine handler performance optimization") (1,500 tokens)

Context Prepared:
  ✓ Instructions loaded (3,200)
  ✓ File located and read (2,000)
  ✓ Knowledge retrieved (1,500)
  
Total: 6,700 tokens
vs Manual: ~15,000 tokens
Savings: 55% ✅
```

### Example 2: Frontend Refactoring

```yaml
Input:
  task: "refactor_component"
  file: "src/components/CatalogFilter.vue"
  goal: "Extract hooks and improve composition"

Orchestration Analysis:
  1. GL-043: frontend-essentials.md + security.md (3,100 tokens)
  2. GL-044: semantic_search("Vue composition patterns") (1,500 tokens)
  3. GL-045: query("Pinia with composition API") (1,500 tokens)

Context Prepared:
  ✓ Instructions loaded (3,100)
  ✓ Pattern search completed (1,500)
  ✓ Knowledge retrieved (1,500)
  
Total: 6,100 tokens
vs Manual: ~13,000 tokens
Savings: 53% ✅
```

### Example 3: Security Review

```yaml
Input:
  task: "security_review"
  file: "backend/Gateway/Store/Controllers/OrderController.cs"
  focus: "Input validation and SQL injection prevention"

Orchestration Analysis:
  1. GL-043: backend-essentials.md + security.md (3,200 tokens)
  2. GL-044: grep("OrderCreate", "OrderUpdate") → read_file(targeted) (2,000 tokens)
  3. GL-045: query("OWASP input validation patterns") (1,500 tokens)

Context Prepared:
  ✓ Instructions loaded (3,200)
  ✓ Code located and reviewed (2,000)
  ✓ Security knowledge retrieved (1,500)
  
Total: 6,700 tokens
vs Manual: ~16,000 tokens
Savings: 58% ✅
```

---

## ✅ Integration Checklist

### For Agents

When starting work, the orchestration should:
- [ ] Analyze file path → Apply GL-043
- [ ] Analyze task type → Apply GL-044
- [ ] Analyze knowledge needs → Apply GL-045
- [ ] Load ONLY necessary context
- [ ] Prepare optimal tool usage patterns
- [ ] Report estimated token efficiency

### Manual Application (Until Full Automation)

```
Before starting work, ask yourself:

1. What file am I working on?
   → Apply GL-043 path-specific loading

2. What do I need from this file?
   → Find location first (grep) → read targeted range

3. What external knowledge do I need?
   → Query KB on-demand, don't pre-load

4. What's my most efficient path to the answer?
   → Use semantic_search when possible
   → Use list_code_usages for references
   → Avoid full file reads
```

---

## 📈 Expected Impact

| Strategy | Baseline | Savings | Combined |
|----------|----------|---------|----------|
| **GL-043** | 18 KB | 50-70% | 50-70% |
| **GL-044** | Full file reads | 40% | 60-75% |
| **GL-045** | 15-20 KB KB articles | 60% | 70-85% |
| **GL-047** (Orchestration) | Manual decisions | 30% (decision overhead) | **75-85%** |

**Expected Monthly Impact**:
```
50 interactions × 10,000 tokens/interaction × 75% savings
= 375,000 tokens saved per month
≈ Eliminate rate limiting pressure entirely! 🎉
```

---

## 🚀 Rollout Timeline

**Phase 1: Awareness (This Week)**
- [ ] Agents aware of GL-043/044/045
- [ ] Manual application of orchestration logic
- [ ] Monitor effectiveness

**Phase 2: Automation (Next Week)**
- [ ] Create decision engine checklist
- [ ] Train agents on orchestration rules
- [ ] Measure actual improvements

**Phase 3: Integration (By Jan 27)**
- [ ] Automatic context orchestration
- [ ] Monitor combined savings
- [ ] Target: 75-85% reduction achieved

---

## 🔄 Continuous Improvement

### Weekly Review
- [ ] Analyze token consumption patterns
- [ ] Identify manual decisions that could be automated
- [ ] Update orchestration rules

### Monthly Audit (with GL-046)
- [ ] Measure actual savings
- [ ] Compare vs projections
- [ ] Identify edge cases
- [ ] Refine decision engine

### Quarterly Optimization
- [ ] Deep analysis of bottlenecks
- [ ] Create specialized routing rules
- [ ] Update agent guidelines
- [ ] Share learnings

---

## 🎓 Example Agents Using GL-047

### @Backend Developer

**Before (Manual)**:
```
1. Open PR with backend code
2. Manually attach backend, security, and general files
3. Manually decide: grep or read?
4. Manually decide: query KB or attach?
5. Context is bloated and inefficient
```

**After (Orchestrated)**:
```
1. Open PR with backend code
2. Orchestration detects: backend file + fix task
3. Auto-loads: backend-essentials + security only
4. Auto-decides: grep → targeted read
5. Auto-decides: query KB for Wolverine pattern
6. Context is minimal and efficient! ✅
```

---

## 📞 Support

### If automation doesn't match your needs:
1. Manually apply GL-043/044/045 rules
2. Report pattern to @SARAH
3. We'll refine orchestration rules

### If you're unsure about a task:
1. Check the decision trees above
2. Apply the closest matching scenario
3. Adjust if needed

---

**Maintained by**: @SARAH  
**Last Updated**: 7. Januar 2026  
**Status**: Ready for Implementation  
**Expected Benefit**: 75-85% token reduction when combined with GL-043/044/045
