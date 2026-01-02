#!/usr/bin/env python3
"""
AI Model Selection Algorithm
Automatically selects the most cost-effective AI model based on task requirements
Part of the Agent Escalation System for cost-effective AI usage
"""

import json
import os
import sys
from enum import Enum
from pathlib import Path
from typing import Dict, List, Optional, Tuple
from dataclasses import dataclass

class TaskComplexity(Enum):
    """Task complexity levels"""
    SIMPLE = "simple"       # Basic code completion, simple refactoring
    MEDIUM = "medium"       # Code review, debugging, documentation
    COMPLEX = "complex"     # Architecture design, complex algorithms, system analysis

class TaskType(Enum):
    """Task type categories"""
    CODE_COMPLETION = "code_completion"
    CODE_REVIEW = "code_review"
    DEBUGGING = "debugging"
    DOCUMENTATION = "documentation"
    TESTING = "testing"
    ARCHITECTURE = "architecture"
    REFACTORING = "refactoring"
    ANALYSIS = "analysis"

@dataclass
class ModelCapability:
    """AI model capabilities and costs"""
    name: str
    cost_per_1k_tokens: float
    max_tokens: int
    strengths: List[str]
    weaknesses: List[str]
    quality_score: float  # 0.0 to 1.0

class ModelSelector:
    """Intelligent model selection based on task requirements and budget"""

    # Model capabilities database (based on 2025 capabilities)
    MODELS = {
        'claude-3-haiku': ModelCapability(
            name='claude-3-haiku',
            cost_per_1k_tokens=0.0000005,  # $0.0005 per 1K tokens
            max_tokens=200000,
            strengths=['fast', 'cost-effective', 'good for simple tasks'],
            weaknesses=['limited reasoning', 'shorter context'],
            quality_score=0.7
        ),
        'gpt-4o-mini': ModelCapability(
            name='gpt-4o-mini',
            cost_per_1k_tokens=0.0000015,  # $0.0015 per 1K tokens
            max_tokens=128000,
            strengths=['balanced performance', 'good code completion'],
            weaknesses=['limited complex reasoning'],
            quality_score=0.75
        ),
        'codellama-34b': ModelCapability(
            name='codellama-34b',
            cost_per_1k_tokens=0.000005,  # $0.005 per 1K tokens
            max_tokens=16000,
            strengths=['open source', 'good for coding', 'long context'],
            weaknesses=['slower', 'less accurate'],
            quality_score=0.65
        ),
        'claude-3-5-sonnet': ModelCapability(
            name='claude-3-5-sonnet',
            cost_per_1k_tokens=0.000015,  # $0.015 per 1K tokens
            max_tokens=200000,
            strengths=['excellent reasoning', 'long context', 'high quality'],
            weaknesses=['expensive', 'slower'],
            quality_score=0.95
        ),
        'gpt-4o': ModelCapability(
            name='gpt-4o',
            cost_per_1k_tokens=0.00003,  # $0.03 per 1K tokens
            max_tokens=128000,
            strengths=['best quality', 'excellent reasoning', 'multimodal'],
            weaknesses=['very expensive', 'rate limited'],
            quality_score=0.98
        ),
        'claude-3-opus': ModelCapability(
            name='claude-3-opus',
            cost_per_1k_tokens=0.000015,  # $0.015 per 1K tokens
            max_tokens=200000,
            strengths=['best reasoning', 'long context', 'high quality'],
            weaknesses=['expensive', 'slower than Sonnet'],
            quality_score=0.96
        )
    }

    def __init__(self, project_root: Path):
        self.project_root = project_root
        self.config_file = project_root / '.ai' / 'config' / 'ai-budget.json'
        self.cache_file = project_root / '.ai' / 'config' / 'model-cache.json'
        self._load_config()

    def _load_config(self):
        """Load budget and optimization configuration"""
        self.budget_config = {
            'monthly_limit': 500.0,
            'daily_limit': 15.0,
            'quality_threshold': 0.8,
            'auto_escalation': False
        }

        if self.config_file.exists():
            try:
                with open(self.config_file, 'r') as f:
                    config = json.load(f)
                    self.budget_config.update(config.get('budget', {}))
                    self.budget_config.update(config.get('optimization', {}))
            except Exception as e:
                print(f"Warning: Could not load config: {e}", file=sys.stderr)

    def select_model(self, task_type: TaskType, complexity: TaskComplexity,
                    max_tokens: int = 4000, quality_requirement: float = 0.8,
                    budget_priority: float = 0.5) -> Dict:
        """
        Select the optimal model based on task requirements

        Args:
            task_type: Type of task being performed
            complexity: Complexity level of the task
            max_tokens: Maximum tokens expected for the task
            quality_requirement: Minimum quality score required (0.0-1.0)
            budget_priority: Priority for cost vs quality (0.0 = cost first, 1.0 = quality first)

        Returns:
            Dictionary with selected model and reasoning
        """
        # Filter models that meet basic requirements
        candidates = []
        for model in self.MODELS.values():
            if model.max_tokens >= max_tokens and model.quality_score >= quality_requirement:
                candidates.append(model)

        if not candidates:
            # Fallback to basic models if no candidates meet requirements
            candidates = [self.MODELS['gpt-4o-mini'], self.MODELS['claude-3-haiku']]

        # Score candidates based on task fit and budget priority
        scored_candidates = []
        for model in candidates:
            # Task fit score (0.0-1.0)
            task_fit = self._calculate_task_fit(model, task_type, complexity)

            # Cost score (normalized, lower cost = higher score)
            max_cost = max(m.cost_per_1k_tokens for m in candidates)
            min_cost = min(m.cost_per_1k_tokens for m in candidates)
            if max_cost == min_cost:
                cost_score = 1.0
            else:
                cost_score = 1.0 - ((model.cost_per_1k_tokens - min_cost) / (max_cost - min_cost))

            # Combined score
            final_score = (budget_priority * cost_score) + ((1 - budget_priority) * task_fit)

            scored_candidates.append((model, final_score, task_fit, cost_score))

        # Sort by final score (highest first)
        scored_candidates.sort(key=lambda x: x[1], reverse=True)

        best_model, score, task_fit, cost_score = scored_candidates[0]

        # Get alternatives
        alternatives = [model.name for model, _, _, _ in scored_candidates[1:3]]

        return {
            'selected_model': best_model.name,
            'confidence_score': round(score, 3),
            'estimated_cost_per_1k': best_model.cost_per_1k_tokens,
            'estimated_cost_for_task': round((max_tokens / 1000) * best_model.cost_per_1k_tokens, 4),
            'quality_score': best_model.quality_score,
            'max_tokens': best_model.max_tokens,
            'task_fit_score': round(task_fit, 3),
            'cost_score': round(cost_score, 3),
            'alternatives': alternatives,
            'reasoning': self._generate_reasoning(best_model, task_type, complexity, budget_priority)
        }

    def _calculate_task_fit(self, model: ModelCapability, task_type: TaskType,
                           complexity: TaskComplexity) -> float:
        """Calculate how well a model fits a specific task"""
        base_score = model.quality_score

        # Adjust based on task type
        task_multipliers = {
            TaskType.CODE_COMPLETION: {
                'claude-3-haiku': 0.9,
                'gpt-4o-mini': 1.0,
                'codellama-34b': 0.95,
                'claude-3-5-sonnet': 0.85,
                'gpt-4o': 0.8,
                'claude-3-opus': 0.75
            },
            TaskType.CODE_REVIEW: {
                'claude-3-haiku': 0.7,
                'gpt-4o-mini': 0.8,
                'codellama-34b': 0.6,
                'claude-3-5-sonnet': 1.0,
                'gpt-4o': 0.95,
                'claude-3-opus': 0.9
            },
            TaskType.DEBUGGING: {
                'claude-3-haiku': 0.75,
                'gpt-4o-mini': 0.85,
                'codellama-34b': 0.7,
                'claude-3-5-sonnet': 0.95,
                'gpt-4o': 1.0,
                'claude-3-opus': 0.9
            },
            TaskType.ARCHITECTURE: {
                'claude-3-haiku': 0.6,
                'gpt-4o-mini': 0.7,
                'codellama-34b': 0.5,
                'claude-3-5-sonnet': 0.9,
                'gpt-4o': 0.95,
                'claude-3-opus': 1.0
            }
        }

        # Adjust based on complexity
        complexity_multipliers = {
            TaskComplexity.SIMPLE: 1.0,
            TaskComplexity.MEDIUM: 0.9,
            TaskComplexity.COMPLEX: 0.8
        }

        task_multiplier = task_multipliers.get(task_type, {}).get(model.name, 0.8)
        complexity_multiplier = complexity_multipliers[complexity]

        return base_score * task_multiplier * complexity_multiplier

    def _generate_reasoning(self, model: ModelCapability, task_type: TaskType,
                           complexity: TaskComplexity, budget_priority: float) -> str:
        """Generate human-readable reasoning for model selection"""
        budget_focus = "cost-effective" if budget_priority > 0.5 else "quality-focused"
        complexity_desc = complexity.value

        reasoning = f"Selected {model.name} for {complexity_desc} {task_type.value} task with {budget_focus} approach. "

        if complexity == TaskComplexity.SIMPLE:
            reasoning += "Simple tasks benefit from faster, cheaper models. "
        elif complexity == TaskComplexity.MEDIUM:
            reasoning += "Medium complexity requires balanced performance and cost. "
        else:
            reasoning += "Complex tasks need high reasoning capability despite higher cost. "

        reasoning += f"Model provides {model.quality_score:.1f} quality score with ${model.cost_per_1k_tokens*1000:.4f}/1K token cost."

        return reasoning

    def get_budget_status(self) -> Dict:
        """Get current budget status and recommendations"""
        # This would integrate with the monitoring system
        # For now, return basic budget info
        return {
            'monthly_budget': self.budget_config['monthly_limit'],
            'daily_budget': self.budget_config['daily_limit'],
            'quality_threshold': self.budget_config['quality_threshold'],
            'auto_escalation': self.budget_config['auto_escalation']
        }

def main():
    import argparse

    parser = argparse.ArgumentParser(description='AI Model Selection Algorithm')
    parser.add_argument('task_type', choices=[t.value for t in TaskType],
                       help='Type of task to perform')
    parser.add_argument('--complexity', choices=[c.value for c in TaskComplexity],
                       default='medium', help='Task complexity level')
    parser.add_argument('--max-tokens', type=int, default=4000,
                       help='Maximum expected tokens for the task')
    parser.add_argument('--quality-min', type=float, default=0.8,
                       help='Minimum quality score required (0.0-1.0)')
    parser.add_argument('--budget-priority', type=float, default=0.5,
                       help='Priority for cost vs quality (0.0=cost first, 1.0=quality first)')
    parser.add_argument('--json', action='store_true',
                       help='Output result as JSON')

    args = parser.parse_args()

    # Initialize selector
    project_root = Path(__file__).parent.parent
    selector = ModelSelector(project_root)

    # Select model
    task_type = TaskType(args.task_type)
    complexity = TaskComplexity(args.complexity)

    result = selector.select_model(
        task_type=task_type,
        complexity=complexity,
        max_tokens=args.max_tokens,
        quality_requirement=args.quality_min,
        budget_priority=args.budget_priority
    )

    if args.json:
        print(json.dumps(result, indent=2))
    else:
        print("🎯 AI Model Selection Result")
        print("=" * 40)
        print(f"Selected Model: {result['selected_model']}")
        print(f"Confidence Score: {result['confidence_score']:.3f}")
        print(f"Estimated Cost: ${result['estimated_cost_for_task']:.4f} ({args.max_tokens} tokens)")
        print(f"Quality Score: {result['quality_score']:.2f}")
        print(f"Task Fit Score: {result['task_fit_score']:.3f}")
        print(f"Cost Score: {result['cost_score']:.3f}")
        if result['alternatives']:
            print(f"Alternatives: {', '.join(result['alternatives'])}")
        print()
        print("📝 Reasoning:")
        print(result['reasoning'])

if __name__ == '__main__':
    main()