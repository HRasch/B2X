#!/usr/bin/env python3
"""
AI Cost Optimization Monitor
Advanced monitoring and optimization for GitHub Copilot usage
Part of the Agent Escalation System

Features:
- Real-time usage tracking
- Cost prediction and optimization
- Model selection recommendations
- Automated escalation based on budget
- Performance analytics
"""

import json
import os
import sys
from datetime import datetime, timedelta
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import argparse
import requests
from dataclasses import dataclass, asdict
import statistics

@dataclass
class AIUsage:
    """Represents a single AI usage event"""
    timestamp: datetime
    agent: str
    model: str
    tokens: int
    cost: float
    task_type: str
    quality_score: Optional[float] = None
    response_time: Optional[float] = None

@dataclass
class BudgetConfig:
    """Budget configuration"""
    monthly_limit: float = 500.0
    daily_limit: float = 15.0
    alert_threshold_80: float = 80.0
    alert_threshold_95: float = 95.0

@dataclass
class OptimizationConfig:
    """Optimization settings"""
    auto_escalation: bool = False
    cache_enabled: bool = True
    batch_processing: bool = False
    quality_threshold: float = 0.8

class AICostMonitor:
    """Advanced AI cost monitoring and optimization system"""

    # Cost per 1K tokens for different models (as of 2025)
    MODEL_COSTS = {
        'gpt-4o': 0.00003,          # $0.03 per 1K tokens
        'gpt-4o-mini': 0.0000015,   # $0.0015 per 1K tokens
        'claude-3-5-sonnet': 0.000015,  # $0.015 per 1K tokens
        'claude-3-haiku': 0.0000005,    # $0.0005 per 1K tokens
        'claude-3-opus': 0.000015,      # $0.015 per 1K tokens
        'gemini-pro': 0.00001,          # Estimated $0.01 per 1K tokens
        'codellama-34b': 0.000005,      # Estimated $0.005 per 1K tokens
    }

    def __init__(self, project_root: Path):
        self.project_root = project_root
        self.config_dir = project_root / '.ai' / 'config'
        self.logs_dir = project_root / 'logs' / 'ai-usage'
        self.config_file = self.config_dir / 'ai-budget.json'
        self.cache_file = self.config_dir / 'ai-cache.json'

        self.config_dir.mkdir(parents=True, exist_ok=True)
        self.logs_dir.mkdir(parents=True, exist_ok=True)

        self.budget_config = self._load_budget_config()
        self.usage_cache: Dict[str, AIUsage] = {}

    def _load_budget_config(self) -> BudgetConfig:
        """Load budget configuration"""
        if self.config_file.exists():
            with open(self.config_file, 'r') as f:
                data = json.load(f)
                budget = data.get('budget', {})
                alerts = data.get('alerts', {})
                return BudgetConfig(
                    monthly_limit=budget.get('monthly', 500.0),
                    daily_limit=budget.get('daily', 15.0),
                    alert_threshold_80=alerts.get('threshold_80_percent', 80.0),
                    alert_threshold_95=alerts.get('threshold_95_percent', 95.0)
                )
        return BudgetConfig()

    def calculate_cost(self, model: str, tokens: int) -> float:
        """Calculate cost for a given model and token count"""
        cost_per_1k = self.MODEL_COSTS.get(model, 0.00001)  # Default fallback
        return (tokens / 1000) * cost_per_1k

    def log_usage(self, agent: str, model: str, tokens: int, task_type: str,
                  quality_score: Optional[float] = None,
                  response_time: Optional[float] = None) -> AIUsage:
        """Log AI usage event"""
        cost = self.calculate_cost(model, tokens)
        usage = AIUsage(
            timestamp=datetime.now(),
            agent=agent,
            model=model,
            tokens=tokens,
            cost=cost,
            task_type=task_type,
            quality_score=quality_score,
            response_time=response_time
        )

        # Save to monthly log
        monthly_file = self.logs_dir / f"{usage.timestamp.strftime('%Y-%m')}.jsonl"
        with open(monthly_file, 'a') as f:
            json.dump(asdict(usage), f, default=str)
            f.write('\n')

        # Update cache
        cache_key = f"{agent}:{task_type}:{usage.timestamp.strftime('%Y-%m-%d %H')}"
        self.usage_cache[cache_key] = usage

        return usage

    def get_usage_stats(self, days: int = 30) -> Dict:
        """Get usage statistics for the last N days"""
        end_date = datetime.now()
        start_date = end_date - timedelta(days=days)

        total_cost = 0.0
        total_tokens = 0
        usage_by_model = {}
        usage_by_agent = {}
        usage_by_task = {}

        # Read log files
        for log_file in self.logs_dir.glob("*.jsonl"):
            try:
                with open(log_file, 'r') as f:
                    for line in f:
                        if line.strip():
                            data = json.loads(line)
                            usage = AIUsage(**data)
                            if start_date <= usage['timestamp'] <= end_date:
                                total_cost += usage['cost']
                                total_tokens += usage['tokens']

                                usage_by_model[usage['model']] = usage_by_model.get(usage['model'], 0) + usage['cost']
                                usage_by_agent[usage['agent']] = usage_by_agent.get(usage['agent'], 0) + usage['cost']
                                usage_by_task[usage['task_type']] = usage_by_task.get(usage['task_type'], 0) + usage['cost']
            except Exception as e:
                print(f"Warning: Error reading {log_file}: {e}", file=sys.stderr)

        return {
            'period_days': days,
            'total_cost': round(total_cost, 2),
            'total_tokens': total_tokens,
            'avg_cost_per_day': round(total_cost / days, 2),
            'usage_by_model': usage_by_model,
            'usage_by_agent': usage_by_agent,
            'usage_by_task': usage_by_task
        }

    def check_budget_alerts(self) -> List[str]:
        """Check budget thresholds and return alerts"""
        alerts = []

        # Get current month usage
        monthly_stats = self.get_usage_stats(days=30)
        monthly_cost = monthly_stats['total_cost']

        # Get today usage
        today_stats = self.get_usage_stats(days=1)
        today_cost = today_stats['total_cost']

        # Monthly alerts
        monthly_percent = (monthly_cost / self.budget_config.monthly_limit) * 100
        if monthly_percent >= self.budget_config.alert_threshold_95:
            alerts.append(f"🚨 CRITICAL: Monthly budget at {monthly_percent:.1f}% (${monthly_cost:.2f}/${self.budget_config.monthly_limit:.2f})")
        elif monthly_percent >= self.budget_config.alert_threshold_80:
            alerts.append(f"⚠️ WARNING: Monthly budget at {monthly_percent:.1f}% (${monthly_cost:.2f}/${self.budget_config.monthly_limit:.2f})")

        # Daily alerts
        daily_percent = (today_cost / self.budget_config.daily_limit) * 100
        if daily_percent >= self.budget_config.alert_threshold_95:
            alerts.append(f"🚨 CRITICAL: Daily budget at {daily_percent:.1f}% (${today_cost:.2f}/${self.budget_config.daily_limit:.2f})")
        elif daily_percent >= self.budget_config.alert_threshold_80:
            alerts.append(f"⚠️ WARNING: Daily budget at {daily_percent:.1f}% (${today_cost:.2f}/${self.budget_config.daily_limit:.2f})")

        return alerts

    def recommend_model(self, task_type: str, complexity: str = 'medium') -> Dict:
        """Recommend the most cost-effective model for a task"""
        recommendations = {
            'simple': ['claude-3-haiku', 'codellama-34b', 'gpt-4o-mini'],
            'medium': ['claude-3-haiku', 'gpt-4o-mini', 'claude-3-5-sonnet'],
            'complex': ['claude-3-5-sonnet', 'gpt-4o', 'claude-3-opus']
        }

        models = recommendations.get(complexity, recommendations['medium'])

        # Get recent performance data for these models
        recent_stats = self.get_usage_stats(days=7)
        model_performance = recent_stats.get('usage_by_model', {})

        # Sort by cost-effectiveness (lower cost preferred)
        sorted_models = sorted(models, key=lambda m: self.MODEL_COSTS.get(m, 999))

        return {
            'recommended_model': sorted_models[0],
            'alternatives': sorted_models[1:],
            'estimated_cost_per_1k_tokens': self.MODEL_COSTS.get(sorted_models[0], 0),
            'reasoning': f"Selected {sorted_models[0]} for {complexity} complexity {task_type} task"
        }

    def generate_report(self) -> str:
        """Generate comprehensive usage report"""
        stats = self.get_usage_stats(days=30)
        alerts = self.check_budget_alerts()

        report = f"""
🤖 AI Cost Optimization Report
Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}

📊 Usage Summary (Last 30 days):
   • Total Cost: ${stats['total_cost']:.2f}
   • Total Tokens: {stats['total_tokens']:,}
   • Average Daily Cost: ${stats['avg_cost_per_day']:.2f}

💰 Budget Status:
   • Monthly Budget: ${self.budget_config.monthly_limit:.2f}
   • Daily Budget: ${self.budget_config.daily_limit:.2f}
   • Monthly Used: {(stats['total_cost'] / self.budget_config.monthly_limit * 100):.1f}%

📈 Usage by Model:
"""

        for model, cost in stats['usage_by_model'].items():
            report += f"   • {model}: ${cost:.2f}\n"

        report += "\n👥 Usage by Agent:\n"
        for agent, cost in stats['usage_by_agent'].items():
            report += f"   • {agent}: ${cost:.2f}\n"

        report += "\n🎯 Usage by Task Type:\n"
        for task, cost in stats['usage_by_task'].items():
            report += f"   • {task}: ${cost:.2f}\n"

        if alerts:
            report += "\n🚨 Budget Alerts:\n"
            for alert in alerts:
                report += f"   • {alert}\n"

        # Optimization recommendations
        report += "\n💡 Optimization Recommendations:\n"
        if stats['avg_cost_per_day'] > self.budget_config.daily_limit * 0.8:
            report += "   • High daily spending detected - consider enabling caching\n"
        if stats['total_cost'] > self.budget_config.monthly_limit * 0.8:
            report += "   • Approaching monthly limit - review agent usage patterns\n"

        # Model recommendations
        simple_rec = self.recommend_model('code_review', 'simple')
        complex_rec = self.recommend_model('architecture', 'complex')

        report += f"   • For simple tasks: Use {simple_rec['recommended_model']} (${simple_rec['estimated_cost_per_1k_tokens']*1000:.4f}/1K tokens)\n"
        report += f"   • For complex tasks: Use {complex_rec['recommended_model']} (${complex_rec['estimated_cost_per_1k_tokens']*1000:.4f}/1K tokens)\n"

        return report

def main():
    parser = argparse.ArgumentParser(description='AI Cost Optimization Monitor')
    parser.add_argument('command', choices=['status', 'log', 'alerts', 'recommend', 'simulate'],
                       help='Command to execute')
    parser.add_argument('--agent', help='Agent name for logging')
    parser.add_argument('--model', help='AI model used')
    parser.add_argument('--tokens', type=int, help='Number of tokens used')
    parser.add_argument('--task-type', help='Type of task performed')
    parser.add_argument('--complexity', choices=['simple', 'medium', 'complex'],
                       default='medium', help='Task complexity for recommendations')
    parser.add_argument('--count', type=int, default=1, help='Number of simulations to run')

    args = parser.parse_args()

    # Initialize monitor
    project_root = Path(__file__).parent.parent
    monitor = AICostMonitor(project_root)

    if args.command == 'status':
        print(monitor.generate_report())

    elif args.command == 'log':
        if not all([args.agent, args.model, args.tokens, args.task_type]):
            print("Error: --agent, --model, --tokens, and --task-type required for logging")
            sys.exit(1)

        usage = monitor.log_usage(args.agent, args.model, args.tokens, args.task_type)
        print(f"✅ Logged usage: {usage.agent} used {usage.model} for {usage.task_type} "
              f"({usage.tokens} tokens, ${usage.cost:.4f})")

    elif args.command == 'alerts':
        alerts = monitor.check_budget_alerts()
        if alerts:
            for alert in alerts:
                print(alert)
        else:
            print("✅ No budget alerts - usage within limits")

    elif args.command == 'recommend':
        rec = monitor.recommend_model(args.task_type or 'general', args.complexity)
        print(f"🎯 Recommended model: {rec['recommended_model']}")
        print(f"💰 Cost: ${rec['estimated_cost_per_1k_tokens']*1000:.4f} per 1K tokens")
        print(f"📝 Reasoning: {rec['reasoning']}")
        if rec['alternatives']:
            print(f"🔄 Alternatives: {', '.join(rec['alternatives'])}")

    elif args.command == 'simulate':
        # Simulate realistic usage patterns
        agents = ['Backend', 'Frontend', 'QA', 'Architect', 'TechLead', 'Security', 'DevOps']
        models = ['gpt-4o', 'gpt-4o-mini', 'claude-3-5-sonnet', 'claude-3-haiku']
        task_types = ['code-review', 'implementation', 'debugging', 'documentation', 'testing']

        import random

        print(f"🎲 Simulating {args.count} AI usage events...")
        for i in range(args.count):
            agent = random.choice(agents)
            model = random.choice(models)
            task_type = random.choice(task_types)
            # Realistic token ranges based on task complexity
            token_ranges = {
                'code-review': (200, 800),
                'implementation': (500, 2000),
                'debugging': (300, 1200),
                'documentation': (400, 1500),
                'testing': (250, 1000)
            }
            min_tokens, max_tokens = token_ranges[task_type]
            tokens = random.randint(min_tokens, max_tokens)

            monitor.log_usage(agent, model, tokens, task_type)

        print(f"✅ Simulation complete - {args.count} events logged")

if __name__ == '__main__':
    main()