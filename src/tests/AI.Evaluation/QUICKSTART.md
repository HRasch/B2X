# AI Evaluation - Quick Start for Teams

**Status**: Phase 1 MVP Complete (11. Januar 2026)  
**For**: @Backend, @DevOps, @QA, @Security

---

## What's Ready Now ✅

Microsoft.Extensions.AI.Evaluation integrated with quality, NLP, and safety evaluators for compliance testing.

**All files in**: `src/tests/AI.Evaluation/`

---

## 👨‍💻 For @Backend: Local Testing

### Run Tests
```bash
cd src/tests/AI.Evaluation
dotnet test -v minimal
```

### Test Coverage
- ✅ Quality evaluators (Relevance, Completeness)
- ✅ NLP evaluators (ROUGE, BLEU)
- ✅ Safety baseline (mock evaluators)
- ✅ Integration tests

### Add Your Own Tests
```csharp
// In QualityAndSafetyEvaluationTests.cs
[Fact]
public async Task ChatCompletion_MyFlow_EvaluatesCorrectly()
{
    var setup = new AIEvaluatorSetup();
    var chatClient = setup.CreateChatClient();
    // Your test logic here
}
```

**See**: `README.md` for extension patterns

---

## 🔄 For @DevOps: CI Integration

### Add to Pipeline
CI workflow ready at `.github/workflows/ai-eval.yml`

Option 1: Use standalone (PR runs separately)
```yaml
# Already created, just enable if needed
```

Option 2: Merge into main `dotnet.yml`
```yaml
- name: AI Evaluation Tests
  run: dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj
```

### Configure Secrets (for real Azure OpenAI)
```
AZURE_OPENAI_ENDPOINT = https://your-resource.openai.azure.com/
AZURE_OPENAI_API_KEY = your-api-key
AZURE_OPENAI_DEPLOYMENT_NAME = gpt-4
```

If not set: Tests use mock (always passes, safe for CI)

### Artifact Retention
- Reports: 30 days (configurable)
- Coverage: Collected automatically
- Logs: Uploaded to PR artifacts

**See**: `.github/workflows/ai-eval.yml`

---

## 🧪 For @QA: Threshold Management

### Current Thresholds
```
MinRelevanceScore       = 0.70   (70%)
MinCompletenessScore    = 0.75   (75%)
MinRougeScore           = 0.50   (50% similarity)
MinBleuScore            = 0.30   (30% n-gram overlap)
SafetyFailureThreshold  = 0.00   (hard fail on harm)
```

**Edit**: `src/tests/AI.Evaluation/AIEvaluatorSetup.cs` → `CreateThresholds()`

### Adjust Thresholds
```csharp
public EvaluationThresholds CreateThresholds() => new()
{
    MinRelevanceScore = 0.75,      // Increase strictness
    MinCompletenessScore = 0.80,   // Increase strictness
    // ... etc
};
```

### Waiver Process
1. Test scores below threshold
2. Author requests waiver in PR with justification
3. You review & approve (or reject)
4. Logged in `.ai/compliance/ai-eval-waivers.md`

**Waiver SLA**:
- 🟡 Yellow (quality 2–5% below): 14 days
- 🟠 Orange (quality >5% below): 7 days
- 🔴 Red (safety): 0 days → **Block, no waiver**

---

## 🔒 For @Security: Safety Violations

### Non-Waiverable
- ❌ Harmful content (violence, hate, unfairness, self-harm, sexual)
- ❌ Protected/copyrighted material
- ❌ Code vulnerabilities (SQL injection, XSS)

**Policy**: These violations **block merges immediately**. No waivers allowed.

### Safety Gate
```
if (ContentHarmEvaluator.Score > 0.0) → BLOCK
if (ProtectedMaterialEvaluator.Score > 0.0) → BLOCK
```

### Setup Azure AI Foundry (Phase 2)
```csharp
// In AIEvaluatorSetup.cs
var endpoint = "https://your-ai-foundry.cognitiveservices.azure.com/";
var apiKey = "your-foundry-api-key";
var contentHarmEval = new ContentHarmEvaluator(
    new Uri(endpoint), 
    new Azure.AzureKeyCredential(apiKey)
);
```

**See**: `README.md` for Azure setup guide

---

## 📋 Quick Links

| Role | Action | Link |
|------|--------|------|
| @Backend | Local test setup | `src/tests/AI.Evaluation/README.md` |
| @Backend | Sample tests | `src/tests/AI.Evaluation/QualityAndSafetyEvaluationTests.cs` |
| @Backend | Extend evaluators | `README.md` → "Extending Evaluators" |
| @DevOps | CI workflow | `.github/workflows/ai-eval.yml` |
| @DevOps | Configure secrets | `.github/workflows/ai-eval.yml` |
| @QA | Thresholds | `src/tests/AI.Evaluation/AIEvaluatorSetup.cs` |
| @QA | Waivers | `.ai/compliance/ai-eval-waivers.md` |
| @Security | Safety config | `README.md` → "Azure AI Foundry" |
| @Security | Policy | `.ai/compliance/ai-eval-waivers.md` |

---

## ⚡ Next Steps (Immediate)

1. **@Backend**: Run `dotnet test` locally; confirm tests pass
2. **@DevOps**: Review `ai-eval.yml`; merge into main pipeline or enable separately
3. **@QA**: Confirm thresholds are acceptable; adjust if needed
4. **@Security**: Review safety gate logic; plan Azure AI Foundry setup

---

## ⏭️ Phase 2 (Future)

- Advanced evaluators: RetrievalEvaluator, GroundednessEvaluator, ToolCallAccuracyEvaluator
- Azure Blob reporting for persistent caching
- Nightly evaluation matrix runs
- Weekly AI quality dashboard

---

## 📞 Questions?

- **Setup**: See `src/tests/AI.Evaluation/README.md`
- **CI issues**: Check `.github/workflows/ai-eval.yml`
- **Thresholds**: Contact @QA
- **Safety**: Contact @Security

---

**Status**: ✅ Ready to go!

