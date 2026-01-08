---
docid: KB-171
title: Github Copilot Models
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: KB-016
title: GitHub Copilot AI Models & Pricing
owner: GitHub Copilot
status: Active
---

# GitHub Copilot AI Models & Pricing

**Version**: Latest as of January 2026  
**Last Updated**: 2026-01-02  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Overview

GitHub Copilot supports multiple AI models from leading providers (OpenAI, Anthropic, Google, xAI), each optimized for different use cases. Models vary in speed, reasoning capabilities, cost, and specialized features like multimodal input support.

## Available AI Models

### OpenAI Models

| Model | Status | Best For | Key Features | Multiplier |
|-------|--------|----------|--------------|------------|
| **GPT-4.1** | GA | General-purpose coding | Fast, accurate completions, explanations | 0 |
| **GPT-5** | GA | Deep reasoning, debugging | Multi-step problem solving, architecture analysis | 1 |
| **GPT-5 mini** | GA | General-purpose coding | Fast, accurate, multimodal (vision) | 0 |
| **GPT-5-Codex** | Public preview | General-purpose coding | Enhanced code generation | 1 |
| **GPT-5.1** | GA | Deep reasoning, debugging | Advanced problem solving | 1 |
| **GPT-5.1-Codex** | GA | General-purpose coding | Code-focused completions | 1 |
| **GPT-5.1-Codex-Mini** | Public preview | Fast coding tasks | Lightweight code generation | 0.33 |
| **GPT-5.1-Codex-Max** | GA | Complex coding | Maximum code quality | 1 |
| **GPT-5.2** | GA | Deep reasoning, debugging | Latest reasoning capabilities | 1 |

### Anthropic Models

| Model | Status | Best For | Key Features | Multiplier |
|-------|--------|----------|--------------|------------|
| **Claude Haiku 4.5** | GA | Fast, simple tasks | Speed-optimized, reliable answers | 0.33 |
| **Claude Sonnet 4** | GA | Deep reasoning, debugging | Balanced performance, multimodal | 1 |
| **Claude Sonnet 4.5** | GA | General-purpose coding | Complex problem-solving, sophisticated reasoning | 1 |
| **Claude Opus 4.1** | GA | Deep reasoning, debugging | Most powerful, complex challenges | 10 |
| **Claude Opus 4.5** | GA | Deep reasoning, debugging | Latest Opus capabilities | 3 |

### Google Models

| Model | Status | Best For | Key Features | Multiplier |
|-------|--------|----------|--------------|------------|
| **Gemini 2.5 Pro** | GA | Deep reasoning, debugging | Complex code generation, research workflows | 1 |
| **Gemini 3 Flash** | Public preview | Fast, simple tasks | Speed-optimized responses | 0.33 |
| **Gemini 3 Pro** | Public preview | Deep reasoning, debugging | Advanced reasoning, technical analysis | 1 |

### Other Models

| Model | Provider | Status | Best For | Key Features | Multiplier |
|-------|----------|--------|----------|--------------|------------|
| **Grok Code Fast 1** | xAI | GA | General-purpose coding | Fast, accurate code completions | 0.25 |
| **Raptor mini** | Fine-tuned GPT-5 mini | Public preview | General-purpose coding | Fast, accurate suggestions | 0 |

## Model Capabilities by Task Type

### General-Purpose Coding & Writing
- **Best Models**: GPT-4.1, GPT-5 mini, GPT-5-Codex, Grok Code Fast 1, Raptor mini
- **Use Cases**: Code writing, documentation, error explanations, non-English programming
- **Characteristics**: Balance of quality, speed, cost-efficiency

### Fast Help with Simple/Repetitive Tasks
- **Best Models**: Claude Haiku 4.5, Gemini 3 Flash
- **Use Cases**: Quick edits, utility functions, syntax help, lightweight prototyping
- **Characteristics**: Optimized for speed and responsiveness

### Deep Reasoning & Debugging
- **Best Models**: GPT-5, GPT-5.1, GPT-5.2, Claude Sonnet 4, Claude Sonnet 4.5, Claude Opus 4.1, Claude Opus 4.5, Gemini 2.5 Pro, Gemini 3 Pro
- **Use Cases**: Complex debugging, large refactoring, architecture planning, multi-file analysis
- **Characteristics**: Step-by-step reasoning, complex decision-making, high-context awareness

### Working with Visuals (Multimodal)
- **Best Models**: GPT-5 mini, Claude Sonnet 4, Claude Opus 4.1, Gemini 2.5 Pro
- **Use Cases**: UI screenshots, diagrams, visual debugging, front-end behavior analysis
- **Characteristics**: Support for image input and visual reasoning

## Pricing Plans

### Free Plan ($0/month)
- **Agent Mode/Chat Requests**: 50 per month
- **Code Completions**: 2,000 per month
- **Available Models**: Claude Haiku 4.5, GPT-4.1, GPT-5 mini, and more
- **Premium Requests**: None included
- **Eligibility**: All users

### Pro Plan ($10/month or $100/year)
- **Agent Mode/Chat**: Unlimited with GPT-5 mini
- **Code Completions**: Unlimited
- **Premium Requests**: 300 per month (can purchase more)
- **Available Models**: All models except some premium ones
- **Free Access**: Available for verified students, teachers, and open source maintainers

### Pro+ Plan ($39/month or $390/year)
- **Everything in Pro** plus:
- **Premium Requests**: 5x more than Pro (1,500 per month)
- **Available Models**: All models including premium ones
- **Additional Features**: GitHub Spark IDE extension support

## Model Multipliers & Cost Impact

Premium request multipliers determine usage consumption:

- **0**: No premium requests used (free tier models)
- **0.25-0.33**: Low-cost models (Claude Haiku 4.5, Gemini 3 Flash, Grok Code Fast 1)
- **1**: Standard premium models (most GPT-5 and Claude Sonnet models)
- **3-10**: High-cost premium models (Claude Opus models)

## Plan Availability by Model

| Model | Free | Pro | Pro+ |
|-------|:----:|:---:|:----:|
| GPT-4.1 | ✅ | ✅ | ✅ |
| GPT-5 mini | ✅ | ✅ | ✅ |
| Claude Haiku 4.5 | ✅ | ✅ | ✅ |
| GPT-5 | ❌ | ✅ | ✅ |
| Claude Sonnet 4 | ❌ | ✅ | ✅ |
| Claude Sonnet 4.5 | ❌ | ✅ | ✅ |
| Claude Opus 4.1 | ❌ | ❌ | ✅ |
| Claude Opus 4.5 | ❌ | ✅ | ✅ |
| Gemini 2.5 Pro | ❌ | ✅ | ✅ |
| Gemini 3 Flash | ❌ | ✅ | ✅ |
| Gemini 3 Pro | ❌ | ✅ | ✅ |
| Grok Code Fast 1 | ❌ | ✅ | ✅ |
| Raptor mini | ✅ | ✅ | ✅ |

## Model Selection Guidelines

### For Cost-Conscious Development
1. **Use Free tier models** for basic coding tasks
2. **Claude Haiku 4.5** for fast, simple queries
3. **GPT-5 mini** for general coding with unlimited usage in Pro

### For Complex Development
1. **GPT-5 or GPT-5.1** for deep reasoning and debugging
2. **Claude Opus 4.1** for maximum reasoning power (Pro+ only)
3. **Gemini 2.5 Pro** for research-heavy workflows

### For Speed-Critical Workflows
1. **Claude Haiku 4.5** for instant responses
2. **Gemini 3 Flash** for fast iterations
3. **Grok Code Fast 1** for coding-focused speed

## Recent Model Changes

### Retired Models (2025)
- Claude Sonnet 3.5 → Replaced by Claude Haiku 4.5
- Claude Opus 4 → Replaced by Claude Opus 4.1
- Claude Sonnet 3.7 → Replaced by Claude Sonnet 4
- Gemini 2.0 Flash → Replaced by Gemini 2.5 Pro
- Various o-series models (o1-mini, o3, etc.) → Replaced by GPT-5 series

### New Models (2025-2026)
- GPT-5.2 (latest reasoning model)
- Claude Opus 4.5 (enhanced Opus)
- Gemini 3 Flash/Pro (latest Google models)
- Raptor mini (specialized fine-tuned model)

## Usage Recommendations for B2X

### Default Model Selection
- **General coding**: GPT-5 mini (unlimited in Pro, good balance)
- **Complex debugging**: GPT-5 or Claude Sonnet 4
- **Architecture decisions**: Claude Opus 4.1 (Pro+ only)
- **Fast prototyping**: Claude Haiku 4.5

### Cost Optimization
- Monitor premium request usage in Pro/Pro+ plans
- Use lower-multiplier models for routine tasks
- Reserve high-multiplier models (Opus 4.1) for critical decisions

### Team Considerations
- **Free tier**: Suitable for basic coding assistance
- **Pro tier**: Recommended for active development teams
- **Pro+ tier**: For teams needing maximum reasoning power and unlimited premium usage

## Model Hosting & Availability

Models are hosted across different providers and may have regional availability. Copilot automatically selects the best available model based on your location and current load.

## Further Reading

- [GitHub Copilot Plans & Pricing](https://github.com/features/copilot/plans)
- [AI Model Comparison Guide](https://docs.github.com/en/copilot/reference/ai-models/model-comparison)
- [Supported AI Models Reference](https://docs.github.com/en/copilot/using-github-copilot/ai-models/supported-ai-models-in-copilot)

---

**Next Review**: March 2026  
**Sources**: GitHub Copilot Documentation (January 2026)