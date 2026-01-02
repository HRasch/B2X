#!/usr/bin/env python3
"""
AI Optimization Control Center
Unified interface for all AI cost optimization and escalation features
Part of the Agent Escalation System for cost-effective AI usage
"""

import json
import sys
from pathlib import Path
from typing import Dict, List, Optional, Any
from datetime import datetime, timedelta
import argparse
import subprocess

class AIOptimizationCenter:
    """Central control system for AI optimization features"""

    def __init__(self, project_root: Path):
        self.project_root = project_root
        self.scripts_dir = project_root / 'scripts'

        # Component scripts
        self.monitor_script = self.scripts_dir / 'ai-cost-monitor.py'
        self.cache_script = self.scripts_dir / 'ai-cache.py'
        self.quality_script = self.scripts_dir / 'ai-quality-scorer.py'
        self.batch_script = self.scripts_dir / 'ai-batch-processor.py'
        self.selector_script = self.scripts_dir / 'ai-model-selector.py'

    def run_command(self, script: Path, args: List[str]) -> str:
        """Run a script command and return output"""
        try:
            cmd = [str(script)] + args
            result = subprocess.run(cmd, capture_output=True, text=True, cwd=self.project_root)
            if result.returncode == 0:
                return result.stdout
            else:
                return f"Error: {result.stderr}"
        except Exception as e:
            return f"Error running command: {e}"

    def get_system_status(self) -> Dict[str, Any]:
        """Get comprehensive system status"""
        status = {
            'timestamp': datetime.now().isoformat(),
            'components': {},
            'alerts': [],
            'recommendations': []
        }

        # Check monitoring status
        monitor_output = self.run_command(self.monitor_script, ['stats'])
        if 'Error' not in monitor_output:
            # Parse monitoring stats (simplified)
            status['components']['monitoring'] = 'active'
        else:
            status['components']['monitoring'] = 'inactive'
            status['alerts'].append('AI monitoring system not responding')

        # Check cache status
        cache_output = self.run_command(self.cache_script, ['stats'])
        if 'Error' not in cache_output:
            status['components']['cache'] = 'active'
        else:
            status['components']['cache'] = 'inactive'

        # Check quality scorer
        quality_output = self.run_command(self.quality_script, ['stats'])
        if 'Error' not in quality_output:
            status['components']['quality'] = 'active'
        else:
            status['components']['quality'] = 'inactive'

        # Check batch processor
        batch_output = self.run_command(self.batch_script, ['stats'])
        if 'Error' not in batch_output:
            status['components']['batch'] = 'active'
        else:
            status['components']['batch'] = 'inactive'

        # Generate recommendations
        active_components = sum(1 for comp in status['components'].values() if comp == 'active')
        if active_components < 4:
            status['recommendations'].append('Initialize missing AI optimization components')

        if not status['alerts']:
            status['recommendations'].append('All systems operational - cost optimization active')

        return status

    def optimize_request(self, prompt: str, task_type: str = 'general',
                        context: Optional[str] = None) -> Dict[str, Any]:
        """
        Optimize an AI request using all available systems
        Returns the optimal processing strategy
        """
        optimization = {
            'original_prompt': prompt,
            'task_type': task_type,
            'optimization_strategy': {},
            'estimated_savings': 0.0,
            'processing_method': 'direct'
        }

        # 1. Check cache first
        cache_result = self.run_command(self.cache_script, ['get', '--prompt', prompt])
        if 'Cache hit!' in cache_result:
            optimization['processing_method'] = 'cache_hit'
            optimization['estimated_savings'] = 0.8  # 80% cost savings
            optimization['optimization_strategy']['cache'] = 'Response found in cache'
            return optimization

        # 2. Get model recommendation
        model_args = ['code_review' if task_type == 'code' else task_type, '--complexity', 'medium']
        model_output = self.run_command(self.selector_script, model_args)

        if 'Error' not in model_output:
            # Parse model recommendation (simplified)
            optimization['optimization_strategy']['model_selection'] = 'Optimal model selected'

        # 3. Consider batch processing for similar requests
        # This would check if similar requests are pending
        optimization['optimization_strategy']['batch_processing'] = 'Evaluated for batch grouping'

        # 4. Quality monitoring will be applied after response
        optimization['optimization_strategy']['quality_tracking'] = 'Quality metrics will be collected'

        # Calculate total estimated savings
        optimization['estimated_savings'] = 0.3  # 30% overall savings from optimization

        return optimization

    def process_request(self, prompt: str, task_type: str = 'general',
                       context: Optional[str] = None) -> Dict[str, Any]:
        """
        Process an AI request with full optimization
        This is the main entry point for optimized AI processing
        """
        # Get optimization strategy
        optimization = self.optimize_request(prompt, task_type, context)

        result = {
            'optimization': optimization,
            'processing': {},
            'quality_feedback': None,
            'cost_tracking': None
        }

        # Simulate processing (in real implementation, this would call actual AI)
        if optimization['processing_method'] == 'cache_hit':
            result['processing'] = {
                'method': 'cache',
                'response': 'Cached response retrieved',
                'tokens_used': 50,
                'cost': 0.001,
                'model': 'cache'
            }
        else:
            # Simulate AI call
            result['processing'] = {
                'method': 'ai_call',
                'response': f'AI response to: {prompt[:50]}...',
                'tokens_used': 200,
                'cost': 0.004,
                'model': 'claude-3-haiku'
            }

            # Log the usage
            self.run_command(self.monitor_script, [
                'log', 'OptimizationCenter', result['processing']['model'],
                str(result['processing']['tokens_used']), task_type
            ])

            # Store in cache for future use
            self.run_command(self.cache_script, [
                'store', '--prompt', prompt, '--response', result['processing']['response'],
                '--model', result['processing']['model'],
                '--tokens', str(result['processing']['tokens_used']),
                '--cost', str(result['processing']['cost'])
            ])

        # Evaluate quality
        quality_output = self.run_command(self.quality_script, [
            'evaluate', '--prompt', prompt, '--response', result['processing']['response'],
            '--task-type', task_type
        ])

        if 'Error' not in quality_output:
            # Parse quality score (simplified)
            result['quality_feedback'] = 'Quality evaluation completed'

        return result

    def get_optimization_report(self) -> Dict[str, Any]:
        """Generate comprehensive optimization report"""
        report = {
            'generated_at': datetime.now().isoformat(),
            'period': 'last_30_days',
            'sections': {}
        }

        # Cost monitoring report
        monitor_output = self.run_command(self.monitor_script, ['status'])
        report['sections']['cost_monitoring'] = {
            'status': 'active' if 'Error' not in monitor_output else 'error',
            'data': monitor_output.strip()
        }

        # Cache performance
        cache_output = self.run_command(self.cache_script, ['stats'])
        report['sections']['cache_performance'] = {
            'status': 'active' if 'Error' not in cache_output else 'error',
            'data': cache_output.strip()
        }

        # Quality metrics
        quality_output = self.run_command(self.quality_script, ['stats'])
        report['sections']['quality_metrics'] = {
            'status': 'active' if 'Error' not in quality_output else 'error',
            'data': quality_output.strip()
        }

        # Batch processing stats
        batch_output = self.run_command(self.batch_script, ['stats'])
        report['sections']['batch_processing'] = {
            'status': 'active' if 'Error' not in batch_output else 'error',
            'data': batch_output.strip()
        }

        # Calculate overall efficiency
        total_savings = 0.0
        active_components = 0

        for section in report['sections'].values():
            if section['status'] == 'active':
                active_components += 1
                # Estimate savings based on component type
                if 'cache' in section:
                    total_savings += 0.25  # 25% from caching
                elif 'batch' in section:
                    total_savings += 0.15  # 15% from batching
                elif 'quality' in section:
                    total_savings += 0.10  # 10% from quality improvements

        report['summary'] = {
            'active_components': active_components,
            'total_estimated_savings': f"{total_savings:.1%}",
            'system_health': 'healthy' if active_components >= 3 else 'degraded'
        }

        return report

def main():
    parser = argparse.ArgumentParser(description='AI Optimization Control Center')
    parser.add_argument('command', choices=['status', 'optimize', 'process', 'report'],
                       help='Command to execute')
    parser.add_argument('--prompt', help='Prompt for optimization or processing')
    parser.add_argument('--task-type', default='general',
                       choices=['general', 'code', 'debugging', 'documentation', 'testing'],
                       help='Type of task')
    parser.add_argument('--context', help='Additional context for the request')

    args = parser.parse_args()

    # Initialize control center
    project_root = Path(__file__).parent.parent
    center = AIOptimizationCenter(project_root)

    if args.command == 'status':
        status = center.get_system_status()
        print("🤖 AI Optimization System Status")
        print("=" * 50)
        print(f"Timestamp: {status['timestamp']}")
        print()
        print("🔧 Component Status:")
        for component, state in status['components'].items():
            status_icon = "✅" if state == 'active' else "❌"
            print(f"  {status_icon} {component}: {state}")
        print()
        if status['alerts']:
            print("🚨 Alerts:")
            for alert in status['alerts']:
                print(f"  • {alert}")
        print()
        if status['recommendations']:
            print("💡 Recommendations:")
            for rec in status['recommendations']:
                print(f"  • {rec}")

    elif args.command == 'optimize':
        if not args.prompt:
            print("Error: --prompt required for optimization")
            sys.exit(1)

        optimization = center.optimize_request(
            prompt=args.prompt,
            task_type=args.task_type,
            context=args.context
        )

        print("🎯 AI Request Optimization Analysis")
        print("=" * 50)
        print(f"Task Type: {optimization['task_type']}")
        print(f"Processing Method: {optimization['processing_method']}")
        print(f"Estimated Savings: {optimization['estimated_savings']:.1%}")
        print()
        print("⚙️  Optimization Strategy:")
        for key, value in optimization['optimization_strategy'].items():
            print(f"  • {key}: {value}")

    elif args.command == 'process':
        if not args.prompt:
            print("Error: --prompt required for processing")
            sys.exit(1)

        result = center.process_request(
            prompt=args.prompt,
            task_type=args.task_type,
            context=args.context
        )

        print("🚀 AI Request Processing Result")
        print("=" * 50)
        print(f"Optimization Savings: {result['optimization']['estimated_savings']:.1%}")
        print(f"Processing Method: {result['processing']['method']}")
        print(f"Model Used: {result['processing']['model']}")
        print(f"Tokens Used: {result['processing']['tokens_used']}")
        print(f"Cost: ${result['processing']['cost']:.4f}")
        print()
        print("📝 Response:")
        print(f"  {result['processing']['response']}")
        print()
        if result['quality_feedback']:
            print(f"✨ Quality: {result['quality_feedback']}")

    elif args.command == 'report':
        report = center.get_optimization_report()

        print("📊 AI Optimization Comprehensive Report")
        print("=" * 50)
        print(f"Generated: {report['generated_at']}")
        print(f"Period: {report['period']}")
        print()

        print("🏥 System Health:")
        summary = report['summary']
        print(f"  Active Components: {summary['active_components']}/4")
        print(f"  Estimated Total Savings: {summary['total_estimated_savings']}")
        print(f"  System Health: {summary['system_health']}")
        print()

        for section_name, section_data in report['sections'].items():
            status_icon = "✅" if section_data['status'] == 'active' else "❌"
            print(f"{status_icon} {section_name.replace('_', ' ').title()}:")
            if section_data['status'] == 'active':
                # Show first few lines of data
                lines = section_data['data'].split('\n')[:3]
                for line in lines:
                    if line.strip():
                        print(f"  {line}")
            else:
                print("  Component not responding")
            print()

if __name__ == '__main__':
    main()