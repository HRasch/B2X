# 🤖 AI Cost Monitoring & Optimization System

**Part of the Agent Escalation System for cost-effective AI usage**

This system provides comprehensive monitoring, optimization, and automatic model selection for GitHub Copilot and other AI usage in the B2Connect project.

## 📊 Features

### 🔍 **AI Cost Monitoring**
- Real-time usage tracking across all agents
- Budget alerts at 80% and 95% thresholds
- Monthly and daily cost reporting
- Token usage analytics by model, agent, and task type

### 🎯 **Intelligent Model Selection**
- Automatic model selection based on task complexity
- Cost vs quality optimization
- Task-specific model recommendations
- Budget-aware decision making

### 📈 **Optimization Features**
- Response caching to reduce redundant requests
- Quality scoring and performance tracking
- Batch processing for similar tasks
- Emergency controls for budget overruns

## 🚀 Quick Start

### 1. Check Current Status
```bash
./scripts/ai-cost-monitor.sh status
```

### 2. Simulate Usage (for testing)
```bash
./scripts/ai-cost-monitor.sh simulate 10
```

### 3. Get Model Recommendations
```bash
# For code review
./scripts/ai-model-selector.py code_review --complexity medium

# For architecture design
./scripts/ai-model-selector.py architecture --complexity complex

# Cost-focused approach
./scripts/ai-model-selector.py debugging --budget-priority 0.8
```

### 4. Log Manual Usage
```bash
./scripts/ai-cost-monitor.sh log Backend gpt-4o 1500 code-review
```

## 📋 Available Scripts

| Script | Purpose | Usage |
|--------|---------|-------|
| `ai-cost-monitor.sh` | Bash monitoring script | Basic monitoring, alerts, simulation |
| `ai-cost-monitor.py` | Python advanced monitor | Detailed analytics, reporting, optimization |
| `ai-model-selector.py` | Model selection algorithm | Automatic model recommendations |

## ⚙️ Configuration

Edit `.ai/config/ai-budget.json` to customize:

```json
{
  "budget": {
    "monthly": 500.0,
    "daily": 15.0
  },
  "alerts": {
    "threshold_80_percent": 80,
    "threshold_95_percent": 95
  },
  "optimization": {
    "quality_threshold": 0.8,
    "auto_escalation": false,
    "cache_enabled": true
  }
}
```

## 📊 Usage Examples

### Daily Monitoring
```bash
# Check alerts
./scripts/ai-cost-monitor.sh alerts

# View configuration
./scripts/ai-cost-monitor.sh config
```

### Advanced Analytics (Python)
```bash
# Get detailed usage report
./scripts/ai-cost-monitor.py status

# Log usage with quality metrics
./scripts/ai-cost-monitor.py log --agent Backend --model gpt-4o --tokens 1200 --task-type code-review

# Get budget status
./scripts/ai-cost-monitor.py alerts
```

### Model Selection
```bash
# JSON output for automation
./scripts/ai-model-selector.py debugging --json

# High quality priority
./scripts/ai-model-selector.py architecture --budget-priority 0.2

# Maximum cost savings
./scripts/ai-model-selector.py code_completion --budget-priority 0.9
```

## 📈 Model Cost Comparison (2025)

| Model | Cost per 1K tokens | Quality Score | Best For |
|-------|-------------------|----------------|----------|
| claude-3-haiku | $0.0005 | 0.7 | Simple tasks, fast completion |
| gpt-4o-mini | $0.0015 | 0.75 | Balanced performance |
| codellama-34b | $0.005 | 0.65 | Open source, coding focused |
| claude-3-5-sonnet | $0.015 | 0.95 | High quality, complex tasks |
| gpt-4o | $0.03 | 0.98 | Best quality, complex reasoning |
| claude-3-opus | $0.015 | 0.96 | Best reasoning, long context |

## 🚨 Alert System

The system automatically alerts when:

- **80% threshold**: Warning - approaching budget limits
- **95% threshold**: Critical - immediate action required
- **Budget exceeded**: Emergency protocols activated

## 🔄 Integration with Agents

### Automatic Model Selection
Agents can query the model selector before making AI requests:

```bash
# Get recommended model for current task
MODEL=$(./scripts/ai-model-selector.py code_review --complexity medium --json | jq -r '.selected_model')
```

### Usage Logging
Agents should log all AI usage:

```bash
# Log after AI interaction
./scripts/ai-cost-monitor.sh log "$AGENT_NAME" "$MODEL_USED" "$TOKENS_USED" "$TASK_TYPE"
```

## 📊 Log Files

- **Monthly logs**: `logs/ai-usage/YYYY-MM.log`
- **Daily logs**: `logs/ai-usage/YYYY-MM-DD.log`
- **Configuration**: `.ai/config/ai-budget.json`

## 🎯 Optimization Strategies

1. **Task Complexity Assessment**: Use appropriate models for task difficulty
2. **Caching**: Reuse similar responses to reduce costs
3. **Batch Processing**: Group related requests
4. **Quality Thresholds**: Balance cost vs quality requirements
5. **Budget Monitoring**: Regular review of usage patterns

## 🔧 Troubleshooting

### No logs appearing
- Check that `logs/ai-usage/` directory exists
- Verify script permissions: `chmod +x scripts/ai-cost-monitor.sh`

### Configuration not loading
- Ensure `.ai/config/ai-budget.json` exists
- Check JSON syntax with `jq .ai/config/ai-budget.json`

### Model selection not working
- Verify Python dependencies
- Check script permissions
- Run with `--help` for usage information

## 📈 Roadmap

### Phase 2: Advanced Optimization
- [ ] Response caching system
- [ ] Quality scoring framework
- [ ] Batch processing automation
- [ ] GitHub Copilot API integration

### Phase 3: Predictive Features
- [ ] ML-based model selection
- [ ] Cost-benefit analysis
- [ ] Automated budget reallocation
- [ ] Performance prediction

## 🤝 Contributing

When adding new features:

1. Update this README
2. Add usage examples
3. Test with simulation data
4. Update configuration schema
5. Document any new dependencies

## 📞 Support

For issues or questions:
- Check the logs in `logs/ai-usage/`
- Review configuration in `.ai/config/`
- Run diagnostic commands with `--help`
- Contact the AI Cost Optimization team

---

**Status**: ✅ Phase 1 Complete - Basic monitoring and model selection operational
**Next**: Phase 2 - Advanced optimization features