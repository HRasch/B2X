# GitHub Copilot Models Reference

**Last Updated**: 30. Dezember 2025  
**Source**: [GitHub Copilot Supported Models](https://docs.github.com/en/copilot/reference/ai-models/supported-models)  
**Purpose**: Reference for selecting the optimal AI model for each agent  
**Audience**: All developers, @process-assistant, @tech-lead  
**Index**: ⬅️ [Back to AI Knowledge Base](./INDEX.md)

---

## 📊 Available Models Overview (December 2025)

> **⚠️ Major Update**: GitHub Copilot now supports **17+ models** from OpenAI, Anthropic, Google, and xAI!

### All Supported Models

| Model | Provider | Status | Multiplier | Free Plan | Pro/Business |
|-------|----------|--------|------------|-----------|--------------|
| **GPT-4.1** | OpenAI | GA | 0x | ✅ Included | ✅ Included |
| **GPT-5** | OpenAI | GA | 1x | ❌ | ✅ Included |
| **GPT-5 mini** | OpenAI | GA | 0x | ✅ Included | ✅ Included |
| **GPT-5-Codex** | OpenAI | Preview | 1x | ❌ | ✅ Included |
| **GPT-5.1** | OpenAI | GA | 1x | ❌ | ✅ Included |
| **GPT-5.1-Codex** | OpenAI | GA | 1x | ❌ | ✅ Included |
| **GPT-5.1-Codex-Mini** | OpenAI | Preview | 0.33x | ❌ | ✅ Included |
| **GPT-5.1-Codex-Max** | OpenAI | GA | 1x | ❌ | ✅ Included |
| **GPT-5.2** | OpenAI | GA | 1x | ❌ | ✅ Included |
| **Claude Haiku 4.5** | Anthropic | GA | 0.33x | ✅ Included | ✅ Included |
| **Claude Sonnet 4** | Anthropic | GA | 1x | ❌ | ✅ Included |
| **Claude Sonnet 4.5** | Anthropic | GA | 1x | ❌ | ✅ Included |
| **Claude Opus 4.1** | Anthropic | GA | 10x | ❌ | ✅ Pro/Business |
| **Claude Opus 4.5** | Anthropic | GA | 3x | ❌ | ✅ Included |
| **Gemini 2.5 Pro** | Google | GA | 1x | ❌ | ✅ Included |
| **Gemini 3 Flash** | Google | Preview | 0.33x | ❌ | ✅ Included |
| **Gemini 3 Pro** | Google | Preview | 1x | ❌ | ✅ Included |
| **Grok Code Fast 1** | xAI | GA | 0.25x | ❌ | ✅ Included |
| **Raptor mini** | Fine-tuned GPT-5 | Preview | 0x | ✅ Included | ✅ Included |

### Quick Reference: Best Value Models

| Use Case | Recommended Model | Multiplier | Why |
|----------|-------------------|------------|-----|
| **Budget (Free)** | Claude Haiku 4.5 | 0.33x | Best quality on free tier |
| **Fast & Cheap** | Grok Code Fast 1 | 0.25x | Fastest, lowest cost |
| **Balanced** | Claude Sonnet 4 | 1x | Excellent quality/cost |
| **Premium Code** | GPT-5.1-Codex | 1x | Optimized for code |
| **Maximum Quality** | Claude Opus 4.5 | 3x | Best reasoning |
| **Deep Reasoning** | Claude Opus 4.1 | 10x | Most capable (expensive) |

---

## 🎯 Model Capabilities (Detailed)

### OpenAI Models

#### GPT-4.1
**Status**: GA | **Multiplier**: 0x (Free)  
**Best For**: General tasks, included in all plans

- ✅ Included in Free tier
- ✅ No premium request cost
- ✅ Good general-purpose model
- ❌ Not optimized for code

---

#### GPT-5 / GPT-5 mini
**Status**: GA | **Multiplier**: 1x / 0x  
**Best For**: Advanced reasoning, code generation

- ✅ GPT-5 mini is FREE (0x multiplier)
- ✅ Strong reasoning capabilities
- ✅ Better than GPT-4 series
- ✅ Good for complex tasks

---

#### GPT-5.1 Series (Recommended for Code)
**Status**: GA | **Multiplier**: 0.33x - 1x  

| Variant | Multiplier | Best For |
|---------|------------|----------|
| GPT-5.1 | 1x | General advanced tasks |
| GPT-5.1-Codex | 1x | **Code generation** (optimized) |
| GPT-5.1-Codex-Mini | 0.33x | Fast code tasks (budget) |
| GPT-5.1-Codex-Max | 1x | Maximum code quality |

**IDE Support**:
- VS Code v1.104.1+
- JetBrains plugin v1.5.61+
- Xcode plugin v0.45.0+
- Eclipse plugin v0.13.0+

---

#### GPT-5.2
**Status**: GA | **Multiplier**: 1x  
**Best For**: Latest OpenAI capabilities

- ✅ Newest OpenAI model
- ✅ Improved reasoning
- ✅ Available in all paid plans

---

### Anthropic Models

#### Claude Haiku 4.5 ⭐ BEST VALUE
**Status**: GA | **Multiplier**: 0.33x  
**Best For**: Budget-conscious quality

- ✅ **Included in FREE tier**
- ✅ Only 0.33x premium cost
- ✅ Excellent for simple-to-medium tasks
- ✅ Fast response times
- ✅ Good code understanding

**Recommended For**:
- `collaboration-monitor`
- `team-assistant`
- `process-controller`
- Simple coordination tasks

---

#### Claude Sonnet 4 / 4.5
**Status**: GA | **Multiplier**: 1x  
**Best For**: Code generation, architecture

| Model | Strengths |
|-------|-----------|
| Claude Sonnet 4 | Excellent code quality, reasoning |
| Claude Sonnet 4.5 | Enhanced capabilities, latest features |

- ✅ 200K context window
- ✅ Superior code understanding
- ✅ Excellent for architecture decisions
- ✅ Strong security analysis
- ✅ Better at following complex instructions

**Recommended For**:
- `backend-developer`
- `frontend-developer`
- `qa-engineer`
- `security-engineer`
- `software-architect`

---

#### Claude Opus 4.1 / 4.5 (Premium)
**Status**: GA | **Multiplier**: 10x / 3x  
**Best For**: Most complex reasoning, critical decisions

| Model | Multiplier | Use Case |
|-------|------------|----------|
| Claude Opus 4.1 | **10x** | Exceptional reasoning (very expensive) |
| Claude Opus 4.5 | **3x** | Top-tier quality (more affordable) |

- ✅ Anthropic's most capable models
- ✅ Best-in-class reasoning
- ✅ Superior complex analysis
- ⚠️ Opus 4.1 costs 10x premium requests
- ⚠️ Use sparingly for critical decisions

**Recommended For**:
- `software-architect` (critical decisions only)
- `tech-lead` (architecture reviews)
- Complex security audits

---

### Google Models

#### Gemini 2.5 Pro
**Status**: GA | **Multiplier**: 1x  
**Best For**: Multimodal, long context

- ✅ Strong multimodal capabilities
- ✅ Good for image + code tasks
- ✅ Competitive with GPT/Claude

---

#### Gemini 3 Flash / Pro
**Status**: Preview | **Multiplier**: 0.33x / 1x  
**Best For**: Fast responses, experimentation

- ✅ Gemini 3 Flash is budget-friendly (0.33x)
- ✅ Latest Google AI capabilities
- ⚠️ Preview status - may change

---

### xAI Models

#### Grok Code Fast 1 ⭐ FASTEST
**Status**: GA | **Multiplier**: 0.25x  
**Best For**: Speed-critical tasks

- ✅ **Lowest cost** (0.25x multiplier)
- ✅ Optimized for speed
- ✅ Good for simple code tasks
- ⚠️ Complimentary access extended (no end date set)

**Recommended For**:
- File watching
- Simple routing
- High-volume low-complexity tasks

---

### Fine-Tuned Models

#### Raptor mini
**Status**: Preview | **Multiplier**: 0x  
**Best For**: Specialized tasks

- ✅ Fine-tuned GPT-5 mini
- ✅ FREE (0x multiplier)
- ⚠️ VS Code only
- ⚠️ Preview - capabilities may vary

---

## �️ Retired Models (Do Not Use)

The following models have been **retired** from GitHub Copilot:

| Model | Retired Date | Replacement |
|-------|--------------|-------------|
| Claude Sonnet 3.5 | 2025-11-06 | Claude Haiku 4.5 |
| Claude Opus 4 | 2025-10-23 | Claude Opus 4.1 |
| Claude Sonnet 3.7 | 2025-10-23 | Claude Sonnet 4 |
| Claude Sonnet 3.7 Thinking | 2025-10-23 | Claude Sonnet 4 |
| Gemini 2.0 Flash | 2025-10-23 | Gemini 2.5 Pro |
| o1-mini | 2025-10-23 | GPT-5 mini |
| o3 | 2025-10-23 | GPT-5 |
| o3-mini | 2025-10-23 | GPT-5 mini |
| o4-mini | 2025-10-23 | GPT-5 mini |

> **⚠️ If you see these models in agent configs, update them immediately!**

---

## 💰 Cost Comparison (Premium Request Multipliers)

### Understanding Multipliers

Each model has a **premium request multiplier** that affects your allowance:

| Multiplier | Meaning | Example |
|------------|---------|---------|
| 0x | FREE - no premium cost | GPT-4.1, GPT-5 mini, Raptor mini |
| 0.25x | Very cheap (4 requests = 1 premium) | Grok Code Fast 1 |
| 0.33x | Budget (3 requests = 1 premium) | Claude Haiku 4.5, Gemini 3 Flash |
| 1x | Standard (1 request = 1 premium) | Claude Sonnet 4/4.5, GPT-5.1 |
| 3x | Premium (1 request = 3 premium) | Claude Opus 4.5 |
| 10x | Expensive (1 request = 10 premium) | Claude Opus 4.1 |

### Cost Tier Summary

| Tier | Models | Multiplier |
|------|--------|------------|
| **FREE** | GPT-4.1, GPT-5 mini, Raptor mini | 0x |
| **Budget** | Grok Code Fast 1 | 0.25x |
| **Economy** | Claude Haiku 4.5, Gemini 3 Flash, GPT-5.1-Codex-Mini | 0.33x |
| **Standard** | Claude Sonnet 4/4.5, GPT-5, GPT-5.1, Gemini 2.5/3 Pro | 1x |
| **Premium** | Claude Opus 4.5 | 3x |
| **Enterprise** | Claude Opus 4.1 | 10x |

### GitHub Copilot Subscription Tiers

| Tier | Monthly Cost | Notes |
|------|--------------|-------|
| **Free** | $0 | Limited models, 50 premium/month |
| **Pro** | $10/month | All models, 300 premium/month |
| **Business** | $19/month | Team features, unlimited* |
| **Enterprise** | $39/month | Admin controls, unlimited* |

*Fair use policy applies

---

## 🎛️ Configuration Strategies (Updated December 2025)

### Strategy 1: Cost-Optimized (Budget)
**Goal**: Minimize costs using FREE and low-multiplier models

```yaml
# FREE Tier (0x multiplier)
collaboration-monitor: gpt-5-mini        # Free, good quality
team-assistant: gpt-5-mini               # Free coordination
process-controller: gpt-4.1              # Free, simple tasks

# Budget Tier (0.25x - 0.33x multiplier)
seo-specialist: claude-haiku-4.5         # 0.33x, good analysis
backend-developer: claude-haiku-4.5      # 0.33x, solid code
frontend-developer: claude-haiku-4.5     # 0.33x, components
qa-engineer: grok-code-fast-1            # 0.25x, fast tests
devops-engineer: grok-code-fast-1        # 0.25x, scripts
ui-expert: claude-haiku-4.5              # 0.33x, design
ux-expert: claude-haiku-4.5              # 0.33x, UX analysis

# Standard Tier (1x multiplier) - Critical only
software-architect: claude-sonnet-4      # 1x, architecture
tech-lead: claude-sonnet-4               # 1x, decisions
security-engineer: claude-sonnet-4       # 1x, security
```

**Estimated Cost**: Mostly FREE, minimal premium usage

---

### Strategy 2: Quality-Optimized (Premium)
**Goal**: Maximum output quality

```yaml
# Coordination (0.33x - 1x)
collaboration-monitor: claude-haiku-4.5  # 0.33x
team-assistant: claude-sonnet-4          # 1x
process-controller: claude-sonnet-4      # 1x
seo-specialist: claude-sonnet-4          # 1x

# Development (1x) - Claude Sonnet for best code
backend-developer: claude-sonnet-4.5     # Latest Claude
frontend-developer: claude-sonnet-4.5    # Latest Claude
qa-engineer: claude-sonnet-4             # Strong testing
devops-engineer: gpt-5.1-codex           # Optimized for code
ui-expert: claude-sonnet-4               # Design reasoning
ux-expert: claude-sonnet-4               # UX analysis
qa-reviewer: claude-sonnet-4             # Code review
cli-developer: gpt-5.1-codex             # CLI patterns

# Architecture (1x - 3x) - Best reasoning
software-architect: claude-opus-4.5      # 3x, top tier
tech-lead: claude-sonnet-4.5             # 1x, decisions
security-engineer: claude-opus-4.5       # 3x, security critical
legal-compliance: claude-sonnet-4        # 1x, compliance
process-assistant: claude-sonnet-4       # 1x, governance
scrum-master: claude-sonnet-4            # 1x, process

# AI/ML (1x)
ai-specialist: gemini-2.5-pro            # Multimodal, reasoning
```

**Estimated Cost**: Higher premium usage, best results

---

### Strategy 3: Balanced (Recommended for B2Connect)
**Goal**: Optimal quality/cost ratio

```yaml
# FREE Tier - Simple tasks
collaboration-monitor: gpt-5-mini        # 0x - file watching

# Budget Tier (0.33x) - High volume tasks
team-assistant: claude-haiku-4.5         # 0.33x
process-controller: claude-haiku-4.5     # 0.33x
seo-specialist: claude-haiku-4.5         # 0.33x

# Standard Tier (1x) - Code generation
backend-developer: claude-sonnet-4       # 1x
frontend-developer: claude-sonnet-4      # 1x
qa-engineer: claude-sonnet-4             # 1x
devops-engineer: gpt-5.1-codex           # 1x (code optimized)
ui-expert: claude-sonnet-4               # 1x
ux-expert: claude-sonnet-4               # 1x
qa-reviewer: claude-sonnet-4             # 1x
cli-developer: gpt-5.1-codex             # 1x

# Architecture Tier (1x) - Critical decisions
software-architect: claude-sonnet-4.5    # 1x (latest)
tech-lead: claude-sonnet-4.5             # 1x
security-engineer: claude-sonnet-4.5     # 1x
legal-compliance: claude-sonnet-4        # 1x
process-assistant: claude-sonnet-4       # 1x
scrum-master: claude-sonnet-4            # 1x

# Reasoning Tier (1x) - Complex tasks
ai-specialist: gemini-2.5-pro            # 1x, multimodal
```

**Estimated Cost**: $19/month (Business tier)
---

## 📋 Model Selection Decision Tree

```
┌─────────────────────────────────────────┐
│ What type of task is this agent doing?  │
└───────────────────┬─────────────────────┘
                    │
    ┌───────────────┼───────────────┐
    ▼               ▼               ▼
┌─────────┐   ┌─────────┐   ┌───────────┐
│ Simple  │   │  Code   │   │ Critical  │
│Routing/ │   │Generate/│   │ Decision/ │
│Monitor  │   │ Review  │   │ Architect │
└────┬────┘   └────┬────┘   └─────┬─────┘
     │             │              │
     ▼             │              │
 gpt-5-mini        │              │
 (FREE, 0x)        │              │
                   ▼              │
           ┌──────────────┐       │
           │ Budget       │       │
           │ Conscious?   │       │
           └──────┬───────┘       │
                  │               │
        ┌─────────┼─────────┐     │
        ▼         ▼         │     │
       YES       NO         │     │
        │         │         │     │
        ▼         ▼         │     │
  claude-haiku  claude-     │     │
    4.5         sonnet-4    │     │
  (0.33x)       (1x)        │     │
                            │     │
              ┌─────────────┘     │
              ▼                   ▼
         gpt-5.1-codex     ┌───────────────┐
         (code optimized)  │ Security/Legal│
                           │ Critical?     │
                           └───────┬───────┘
                                   │
                          ┌────────┼────────┐
                          ▼        ▼        ▼
                         YES      NO       AI/ML
                          │        │         │
                          ▼        ▼         ▼
                    claude-   claude-    gemini-
                    opus-4.5  sonnet-4.5 2.5-pro
                    (3x)      (1x)       (1x)
```

---

## 🔧 How to Update Agent Models

### Valid Model Values (December 2025)

```yaml
# OpenAI Models
model: 'gpt-4.1'              # Free, general
model: 'gpt-5'                # Advanced
model: 'gpt-5-mini'           # Free, capable
model: 'gpt-5.1'              # Latest standard
model: 'gpt-5.1-codex'        # Code optimized
model: 'gpt-5.1-codex-mini'   # Budget code
model: 'gpt-5.1-codex-max'    # Maximum code
model: 'gpt-5.2'              # Newest

# Anthropic Models
model: 'claude-haiku-4.5'     # Budget quality
model: 'claude-sonnet-4'      # Standard quality
model: 'claude-sonnet-4.5'    # Latest Sonnet
model: 'claude-opus-4.1'      # Premium (10x)
model: 'claude-opus-4.5'      # Premium (3x)

# Google Models
model: 'gemini-2.5-pro'       # Multimodal
model: 'gemini-3-flash'       # Fast (Preview)
model: 'gemini-3-pro'         # Quality (Preview)

# xAI Models
model: 'grok-code-fast-1'     # Fastest (0.25x)

# Fine-tuned
model: 'raptor-mini'          # Free (Preview)
```

### In Agent File (.github/agents/*.agent.md)

```yaml
---
description: 'Agent description'
tools: ['agent', 'execute', 'vscode']
model: 'claude-sonnet-4'  # ← Update to new model
infer: true
---
```

---

## 📊 B2Connect Current Configuration

> **Note**: These assignments may need updates based on the new December 2025 models

| Agent | Current Model | Multiplier | Status | Recommended |
|-------|---------------|------------|--------|-------------|
| collaboration-monitor | gpt-4o-mini | ⚠️ Retired | ❌ Update | gpt-5-mini |
| team-assistant | gpt-4o | ⚠️ Retired | ❌ Update | claude-haiku-4.5 |
| process-controller | gpt-4o | ⚠️ Retired | ❌ Update | claude-haiku-4.5 |
| seo-specialist | gpt-4o | ⚠️ Retired | ❌ Update | claude-haiku-4.5 |
| backend-developer | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| frontend-developer | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| qa-engineer | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| devops-engineer | claude-sonnet-4 | 1x | ✅ OK | gpt-5.1-codex |
| ui-expert | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| ux-expert | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| qa-reviewer | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| cli-developer | claude-sonnet-4 | 1x | ✅ OK | gpt-5.1-codex |
| software-architect | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4.5 |
| tech-lead | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4.5 |
| security-engineer | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4.5 |
| legal-compliance | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| process-assistant | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| scrum-master | claude-sonnet-4 | 1x | ✅ OK | claude-sonnet-4 |
| ai-specialist | o1-mini | ⚠️ Retired | ❌ Update | gemini-2.5-pro |

### ⚠️ Action Required

The following models are **retired** and need updates:
- `gpt-4o-mini` → Use `gpt-5-mini` (FREE) or `claude-haiku-4.5` (0.33x)
- `gpt-4o` → Use `gpt-5` or `claude-sonnet-4`
- `o1-mini` → Use `gpt-5-mini` or `gemini-2.5-pro`

---

## 🔗 Related Documentation

- [GitHub Copilot Supported Models](https://docs.github.com/en/copilot/reference/ai-models/supported-models) - Official source
- [Agent Index](../../.github/AGENTS_INDEX.md) - All agents and their roles
- [Process Assistant](../../.github/agents/process-assistant.agent.md) - Model update authority
- [C# 14 Features](./C%2314_FEATURES_REFERENCE.md) - Language features for backend
- [daisyUI v5 Components](./DAISYUI_V5_COMPONENTS_REFERENCE.md) - Frontend components

---

## 📝 Changelog

| Date | Change | Author |
|------|--------|--------|
| 2025-12-30 | **Major Update**: Refreshed with GitHub docs (17+ models, retired models, multipliers) | @process-assistant |
| 2025-12-30 | Added: GPT-5 series, Gemini 3, Grok, Opus 4.1/4.5, retired model list | @process-assistant |
| 2025-12-30 | Updated pricing with official multipliers (0x-10x) | @process-assistant |
| 2025-12-30 | Initial document creation | @process-assistant |

---

**Source**: [GitHub Copilot Supported Models Documentation](https://docs.github.com/en/copilot/reference/ai-models/supported-models)  
**Maintained By**: @process-assistant  
**Review Frequency**: Monthly or when new models released  
**Authority**: Model assignments require @process-assistant approval
