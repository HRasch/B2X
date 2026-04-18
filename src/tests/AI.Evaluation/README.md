# AI Evaluation Integration Guide

## Overview

This document describes the .NET AI Evaluation integration for quality and safety testing of AI responses across the B2X platform.

**Status**: Phase 1 (MVP) - Quality + NLP + Safety evaluators integrated with xUnit test runner  
**Location**: `src/tests/AI.Evaluation/`  
**Owner**: @QA, @Security  
**Last Updated**: 11. Januar 2026

---

## What's Included

### Evaluators

1. **Quality Evaluators** (LLM-based)
   - `RelevanceEvaluator`: Measures how well response addresses the query
   - `CompletenessEvaluator`: Measures comprehensiveness and accuracy

2. **NLP Evaluators** (Traditional NLP, no LLM)
   - `RougeEvaluator`: ROUGE-L similarity metric vs. reference text
   - `BleuEvaluator`: BLEU n-gram precision metric vs. reference

3. **Safety Evaluators** (Azure AI Foundry)
   - `ContentHarmEvaluator`: Checks for hate, unfairness, self-harm, violence, sexual content
   - `ProtectedMaterialEvaluator`: Detects protected/copyrighted content
   - (Additional safety evaluators available via Azure)

### Configuration

**File**: `AIEvaluatorSetup.cs`
- Centralizes evaluator initialization and configuration
- Supports both Azure OpenAI and mock fallback
- Defines `EvaluationThresholds` for pass/fail criteria

**Thresholds** (see `EvaluationThresholds` class):
```csharp
MinRelevanceScore = 0.7              // 70% relevance minimum
MinCompletenessScore = 0.75          // 75% completeness minimum
MinRougeScore = 0.5                  // 50% similarity to reference (ROUGE-L)
MinBleuScore = 0.3                   // 30% n-gram overlap (BLEU)
SafetyFailureThreshold = 0.0         // Hard fail: no harmful content
ProtectedMaterialThreshold = 0.0     // Hard fail: no protected material
```

### Test Project

**Project**: `src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj`
- Uses xUnit 3 (MTP v2) for .NET 10 compatibility
- Includes sample tests in `QualityAndSafetyEvaluationTests.cs`
- Test classes:
  - `QualityAndSafetyEvaluationTests`: Quality + safety flow
  - `NLPSimilarityEvaluationTests`: NLP-based evaluation
  - `EvaluationIntegrationTests`: Integration & thresholds

---

## Local Testing

### Prerequisites

```bash
# Install dotnet 10.0
dotnet --version

# Restore packages (includes evaluation libraries)
dotnet restore
```

### Run Tests Locally

```bash
# Run all AI evaluation tests
dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj -v minimal

# Run with coverage
dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj --collect:"XPlat Code Coverage" --logger:"console;verbosity=detailed"

# Run specific test class
dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj --filter "ClassName=QualityAndSafetyEvaluationTests"

# Run with Azure OpenAI (requires credentials)
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-api-key"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4"
dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj
```

### Environment Variables

**For Azure OpenAI Integration**:
```bash
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_API_KEY=<your-key>
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4  # or gpt-35-turbo
```

**For Local Testing** (mock mode):
- If env vars not set, tests use `MockChatClient` (always succeeds, safe for CI)

---

## CI Integration

### Add to CI Pipeline

**File**: `.github/workflows/dotnet.yml` (or equivalent)

```yaml
- name: AI Evaluation Tests
  id: ai-eval
  run: |
    dotnet test src/tests/AI.Evaluation/B2X.Tests.AI.Evaluation.csproj \
      -v minimal \
      --collect:"XPlat Code Coverage" \
      --logger:"console;verbosity=detailed" \
      --results-directory ./test-results
  continue-on-error: true
  env:
    # Use mock if Azure creds unavailable
    AZURE_OPENAI_ENDPOINT: ${{ secrets.AZURE_OPENAI_ENDPOINT }}
    AZURE_OPENAI_API_KEY: ${{ secrets.AZURE_OPENAI_API_KEY }}
    AZURE_OPENAI_DEPLOYMENT_NAME: gpt-4

- name: Generate AI Eval Report
  if: always()
  run: |
    mkdir -p reports/ai-eval
    dotnet aieval report \
      --input test-results \
      --output reports/ai-eval/report.html
  continue-on-error: true

- name: Upload AI Eval Artifacts
  if: always()
  uses: actions/upload-artifact@v3
  with:
    name: ai-eval-report
    path: reports/ai-eval/
    retention-days: 30

- name: Comment PR with AI Eval Results
  if: always() && github.event_name == 'pull_request'
  uses: actions/github-script@v6
  with:
    script: |
      const fs = require('fs');
      const reportPath = 'reports/ai-eval/report.html';
      if (fs.existsSync(reportPath)) {
        github.rest.issues.createComment({
          issue_number: context.issue.number,
          owner: context.repo.owner,
          repo: context.repo.repo,
          body: '📊 AI Evaluation Report: [View Report](https://github.com/${{ github.repository }}/actions/runs/${{ github.run_id }})'
        });
      }
```

### Gate Rules

| Condition | Action |
|-----------|--------|
| Safety violations (harmful content) | ❌ **Block merge** |
| Relevance score < 0.7 | ⚠️ Warn, allow with approval |
| Completeness score < 0.75 | ⚠️ Warn, allow with approval |
| NLP scores < threshold | ⚠️ Warn, allow with approval |

---

## Extending Evaluators

### Add a Custom Quality Evaluator

```csharp
// In AIEvaluatorSetup.cs
public class CustomRelevanceEvaluator : IEvaluator
{
    public async Task<EvaluatorResult> EvaluateAsync(EvaluationInput input)
    {
        // Custom logic to measure relevance
        var score = CalculateRelevance(input.Query, input.Response);
        return new EvaluatorResult { Score = score };
    }

    private double CalculateRelevance(string query, string response)
    {
        // Implement custom relevance logic
        return 0.85;
    }
}

// Add to CreateQualityEvaluators()
public QualityEvaluators CreateQualityEvaluators(IChatClient chatClient)
{
    return new QualityEvaluators(
        RelevanceEvaluator: new CustomRelevanceEvaluator(),
        CompletenessEvaluator: new CompletenessEvaluator(chatClient)
    );
}
```

### Add Safety Evaluator with Azure AI Foundry

```csharp
// Requires Azure subscription and AI Foundry setup
public SafetyEvaluators CreateSafetyEvaluators(string endpoint, string apiKey)
{
    var contentHarmEval = new ContentHarmEvaluator(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
    var protectedMatEval = new ProtectedMaterialEvaluator(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
    
    return new SafetyEvaluators(contentHarmEval, protectedMatEval);
}
```

---

## Compliance & Reporting

### Test Results Retention

- Local artifacts: Retained for 7 days
- CI artifacts: Retained for 30 days (configurable)
- Summary published to PR checks and daily dashboard

### Waiver Policy

Safety violations (harmful content) **cannot be waivered**. Quality thresholds can be waivered with documented justification:

**File**: `.ai/compliance/ai-eval-waivers.md`

```markdown
## Waiver: [Test Name]
- **Test**: QualityAndSafetyEvaluationTests::ChatCompletion_ShouldReturnRelevantResponse
- **Score**: 0.68 (threshold: 0.7)
- **Reason**: LLM model variance; actual response is relevant but scores 2% below
- **Owner**: @QA
- **Expiry**: 2026-02-11
- **Evidence**: PR #1234
```

---

## Thresholds & Policy

### Quality Gate Sequence

1. ✅ Build succeeds
2. ✅ Unit tests pass
3. ✅ AI eval tests pass (all quality + NLP)
4. ✅ Safety violations = 0
5. ✅ Code coverage maintained
6. 🔒 Merge allowed

### Fail Conditions

| Condition | Severity | Action |
|-----------|----------|--------|
| `RelevanceEvaluator.Score < 0.7` | 🟡 Medium | Warn, QA approval required |
| `CompletenessEvaluator.Score < 0.75` | 🟡 Medium | Warn, QA approval required |
| `ProtectedMaterialEvaluator` detects material | 🔴 Critical | Block, waiver not allowed |
| `ContentHarmEvaluator` detects harm | 🔴 Critical | Block, waiver not allowed |

---

## Roadmap (Phase 2+)

### Phase 2: Advanced Evaluators
- `RetrievalEvaluator` for RAG evaluation
- `GroundednessEvaluator` for source attribution
- `ToolCallAccuracyEvaluator` for tool invocation validation
- `CodeVulnerabilityEvaluator` for code-gen flows

### Phase 3: Reporting & Dashboards
- Automated SBOM + evaluation score tracking
- Nightly full evaluator matrix run
- Weekly AI quality & safety dashboard

### Phase 4: Production Monitoring
- Real-time telemetry for production AI flows
- Anomaly detection (evaluation score regression)
- Auto-escalation on safety violations

---

## References

- **Microsoft Docs**: [microsoft.extensions.ai.evaluation](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- **Samples**: [dotnet/ai-samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation)
- **ADR**: See `.ai/decisions/ADR-*.md` for design decisions
- **KB**: See `.ai/knowledgebase/KB-016.md` GitHub Copilot Models

---

## Contact

- **Questions**: Ask @QA or @Backend
- **Issues**: Create GitHub issue with label `ai-eval`
- **Waivers**: Contact @Security for safety violations
