---
docid: KB-176
title: Local Llm Models 2025
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: KB-018
title: Beste lokale LLM-Modelle für Entwicklung (2025/2026)
owner: GitHub Copilot
status: Active
---

# Beste lokale LLM-Modelle für Entwicklung (2025/2026)

**Version**: Januar 2026  
**Last Updated**: 2026-01-02  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Übersicht

Diese Dokumentation listet die aktuell besten lokalen LLM-Modelle für Softwareentwicklung auf, speziell optimiert für Ollama-basierte Deployments auf Hardware wie RTX 5090 (24GB VRAM), 64GB RAM.

## 🏆 Top-Empfehlungen nach Kategorie

### 1. Code-Generierung & Entwicklung

| Modell | Größe | VRAM | Stärken | Ollama-Command |
|--------|-------|------|---------|----------------|
| **DeepSeek-Coder-V2** | 16B/236B | 8-16GB | Code-Generierung auf GPT-4 Niveau | `ollama run deepseek-coder-v2` |
| **Qwen2.5-Coder** | 7B-32B | 4-20GB | Multi-Sprachen, Code-Completion | `ollama run qwen2.5-coder:32b` |
| **DeepSeek-Coder** | 33B | 17GB | Tiefes Code-Verständnis | `ollama run deepseek-coder:33b` |
| **CodeLlama** | 7B-70B | 4-43GB | Meta's Code-Spezialist | `ollama run codellama:34b` |
| **Devstral** | 24B | 14GB | Mistral's Coding-Agent | `ollama run devstral` |

**Empfehlung für B2X (RTX 5090):**
- **Primary**: `deepseek-coder:33b` - Bereits installiert, exzellente Code-Qualität
- **Secondary**: `qwen2.5-coder:32b` - Für Multi-Sprachen Support

### 2. Reasoning & Komplexe Aufgaben

| Modell | Größe | VRAM | Stärken | Ollama-Command |
|--------|-------|------|---------|----------------|
| **DeepSeek-R1** | 7B-671B | 5-404GB | OpenAI O3-Level Reasoning | `ollama run deepseek-r1` |
| **QwQ** | 32B | 20GB | Qwen's Reasoning-Modell | `ollama run qwq` |
| **Qwen3** | 8B-235B | 5-145GB | Thinking + Tools | `ollama run qwen3:30b` |
| **Phi4-Reasoning** | 14B | 9GB | Microsoft's Reasoning | `ollama run phi4-reasoning` |

**Empfehlung für B2X:**
- **Primary**: `qwen3:30b` (bereits installiert) - Thinking-Modus, Tool-Support
- **Alternative**: `deepseek-r1:14b` für komplexe Analysen

### 3. Schnelle Aufgaben & Chat

| Modell | Größe | VRAM | Speed | Ollama-Command |
|--------|-------|------|-------|----------------|
| **Llama3:8b** | 8B | 4.7GB | ~0.6s | `ollama run llama3:8b` |
| **Gemma3** | 4B-27B | 3-17GB | Sehr schnell | `ollama run gemma3` |
| **Phi4-Mini** | 3.8B | 2.5GB | Ultraschnell | `ollama run phi4-mini` |
| **Qwen3:4B** | 4B | 2.5GB | Schnell, kompakt | `ollama run qwen3:4b` |

**Empfehlung für B2X:**
- **Primary**: `llama3:8b` (bereits installiert) - ~0.6s Response, gut für einfache Tasks

### 4. Multimodale Modelle (Vision)

| Modell | Größe | VRAM | Stärken | Ollama-Command |
|--------|-------|------|---------|----------------|
| **Llama3.2-Vision** | 11B/90B | 8-55GB | UI Screenshots, Diagramme | `ollama run llama3.2-vision` |
| **LLaVA** | 7B-34B | 5-22GB | General-Purpose Vision | `ollama run llava` |
| **MiniCPM-V** | 8B | 5GB | Effiziente Vision | `ollama run minicpm-v` |

### 5. Embedding-Modelle (für RAG)

| Modell | Größe | Stärken | Ollama-Command |
|--------|-------|---------|----------------|
| **nomic-embed-text** | 137M | High-Performance | `ollama run nomic-embed-text` |
| **mxbai-embed-large** | 335M | State-of-the-art | `ollama run mxbai-embed-large` |
| **bge-m3** | 567M | Multilingual | `ollama run bge-m3` |

---

## 🎯 B2X Dev-Node Konfiguration

### Aktuell installierte Modelle (192.168.1.117)

| Modell | Größe | Empfohlener Einsatz |
|--------|-------|---------------------|
| `deepseek-coder:33b` | 17.5GB | Komplexe Code-Generierung, Refactoring |
| `deepseek-coder-v2` | 8.3GB | Schnelle Code-Tasks |
| `llama3:8b` | 4.3GB | Schnelle Antworten, Chat, einfache Tasks |
| `qwen3:30b` | 17.3GB | Reasoning, Thinking-Mode, komplexe Analysen |

**Gesamt**: ~47GB Modelle

### Empfohlene Ergänzungen

```bash
# Code-Spezialist (Mistral)
ollama pull devstral

# Reasoning (DeepSeek)
ollama pull deepseek-r1:14b

# Vision für UI-Analyse
ollama pull llama3.2-vision

# Embeddings für RAG
ollama pull nomic-embed-text

# Schnelles Modell
ollama pull phi4-mini
```

---

## 📊 Benchmark-Referenzen (Januar 2026)

### Coding Benchmarks (HumanEval)

| Modell | HumanEval | MBPP | Empfehlung |
|--------|-----------|------|------------|
| DeepSeek-Coder-V2 236B | 90.2% | 87.6% | ⭐ Best |
| Qwen2.5-Coder 32B | 88.4% | 84.5% | ⭐ Excellent |
| DeepSeek-Coder 33B | 81.1% | 79.3% | ✅ Great |
| CodeLlama 34B | 62.2% | 67.5% | ✅ Good |

### Reasoning Benchmarks

| Modell | MATH | GSM8K | Empfehlung |
|--------|------|-------|------------|
| DeepSeek-R1 671B | 97.3% | 97.6% | ⭐ Best (too large) |
| DeepSeek-R1 32B | 89.1% | 92.3% | ⭐ Excellent |
| QwQ 32B | 90.6% | 94.5% | ⭐ Excellent |
| Qwen3 30B | 85.2% | 89.8% | ✅ Great |

### Speed vs Quality Matrix

| Modell | Qualität | Speed | VRAM | Use Case |
|--------|----------|-------|------|----------|
| DeepSeek-Coder:33b | ⭐⭐⭐⭐⭐ | ⭐⭐ | 17GB | Complex code, reviews |
| Qwen3:30b | ⭐⭐⭐⭐ | ⭐⭐⭐ | 17GB | Reasoning, analysis |
| Llama3:8b | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 4.7GB | Quick tasks, chat |
| Phi4-Mini | ⭐⭐ | ⭐⭐⭐⭐⭐ | 2.5GB | Ultra-fast drafts |

---

## 🔧 Task-Routing Strategie

### Automatisches Routing für B2X

```python
def select_model(task_type: str, complexity: str) -> str:
    """Wählt optimales Modell basierend auf Task."""
    
    routing = {
        # Einfache Tasks → Schnelle Modelle
        ("code", "low"): "llama3:8b",
        ("chat", "low"): "llama3:8b",
        ("summary", "low"): "llama3:8b",
        
        # Mittlere Komplexität
        ("code", "medium"): "deepseek-coder-v2",
        ("analysis", "medium"): "qwen3:30b",
        
        # Hohe Komplexität
        ("code", "high"): "deepseek-coder:33b",
        ("reasoning", "high"): "qwen3:30b",
        ("refactor", "high"): "deepseek-coder:33b",
        ("architecture", "high"): "qwen3:30b",
    }
    
    return routing.get((task_type, complexity), "llama3:8b")
```

---

## 📈 Neue Modelle (Q4 2025 - Q1 2026)

### Kürzlich veröffentlicht

| Modell | Release | Highlights |
|--------|---------|------------|
| **Llama 4 Scout** | Q4 2025 | 10M Context, Meta's neuestes |
| **DeepSeek V3.2** | Dez 2025 | 671B MoE, $0.32/M tokens |
| **Gemma 3** | Q4 2025 | Google's effizientes Modell |
| **Qwen3 Coder** | Q3 2025 | Bis 480B Parameter |
| **Devstral 2** | Q4 2025 | 123B Coding-Agent |
| **GLM-4.7** | Jan 2026 | Z.AI's neuestes |

### Coming Soon

| Modell | ETA | Erwartung |
|--------|-----|-----------|
| Llama 4 Maverick | Q1 2026 | 400B multimodal |
| DeepSeek V4 | Q1 2026 | Verbesserte Reasoning |
| Claude Haiku 5 | Q1 2026 | Schneller, günstiger |

---

## 🛠️ Hardware-Empfehlungen

### RTX 5090 (24GB VRAM) - Optimal für:

| Modelle | Quantisierung | Performance |
|---------|---------------|-------------|
| Bis 33B | FP16 | ⭐⭐⭐⭐⭐ Exzellent |
| Bis 70B | Q4_K_M | ⭐⭐⭐⭐ Sehr gut |
| Bis 120B | Q3_K | ⭐⭐⭐ Gut |

### Mit 64GB System RAM:

- CPU-Offloading für größere Modelle möglich
- Hybrid GPU/CPU Inferenz
- Multiple Modelle parallel ladbar

---

## 🔗 Ressourcen

### Offizielle Dokumentation
- [Ollama Model Library](https://ollama.com/library)
- [Ollama GitHub](https://github.com/ollama/ollama)
- [HuggingFace Leaderboard](https://huggingface.co/spaces/open-llm-leaderboard/open_llm_leaderboard)

### Community
- [r/LocalLLaMA](https://www.reddit.com/r/LocalLLaMA/) - Aktive Community
- [Artificial Analysis Leaderboard](https://artificialanalysis.ai/leaderboards/models)

### B2X Spezifisch
- Dev-Node Setup: `tools/dev-node/SETUP_GUIDE.md`
- Dev-Node Script: `scripts/dev-node.py`

---

## Changelog

### 2026-01-02
- Initial version created
- Added benchmark data from artificialanalysis.ai
- Included B2X dev-node recommendations
- Added task routing strategy

---

**Maintained By**: GitHub Copilot  
**Next Review**: 2026-02-01
