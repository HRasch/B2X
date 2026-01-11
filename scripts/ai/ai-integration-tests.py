#!/usr/bin/env python3
"""
AI Optimization Integration Tests
Comprehensive testing suite for the Agent Escalation System
Validates all components working together for cost-effective AI usage
"""

import json
import sys
import time
from pathlib import Path
from typing import Dict, List, Any
from datetime import datetime
import subprocess
import tempfile

class AIOptimizationTester:
    """Integration test suite for AI optimization system"""

    def __init__(self, project_root: Path):
        self.project_root = project_root
        self.scripts_dir = project_root / 'scripts'
        self.test_results = []
        self.start_time = datetime.now()

    def run_command(self, script: Path, args: List[str], timeout: int = 30) -> Dict[str, Any]:
        """Run a command and return structured result"""
        result = {
            'command': f"{script.name} {' '.join(args)}",
            'success': False,
            'output': '',
            'error': '',
            'duration': 0
        }

        start_time = time.time()
        try:
            cmd = [str(script)] + args
            proc = subprocess.run(
                cmd,
                capture_output=True,
                text=True,
                cwd=self.project_root,
                timeout=timeout
            )
            result['success'] = proc.returncode == 0
            result['output'] = proc.stdout
            result['error'] = proc.stderr
        except subprocess.TimeoutExpired:
            result['error'] = f"Command timed out after {timeout} seconds"
        except Exception as e:
            result['error'] = str(e)
        finally:
            result['duration'] = time.time() - start_time

        return result

    def test_component_availability(self) -> bool:
        """Test 1: Check all components are available and executable"""
        print("🧪 Test 1: Component Availability")
        print("-" * 40)

        components = [
            ('ai-cost-monitor.py', ['--help']),
            ('ai-cache.py', ['--help']),
            ('ai-quality-scorer.py', ['--help']),
            ('ai-batch-processor.py', ['--help']),
            ('ai-model-selector.py', ['--help']),
            ('ai-optimization-center.py', ['--help'])
        ]

        # Store individual component results
        component_results = []
        all_available = True
        for script_name, args in components:
            script_path = self.scripts_dir / script_name
            result = self.run_command(script_path, args)

            status = "✅ PASS" if result['success'] else "❌ FAIL"
            print(f"  {status} {script_name}")

            component_results.append({
                'component': script_name,
                'success': result['success'],
                'error': result['error'][:100] if not result['success'] else None
            })

            if not result['success']:
                print(f"    Error: {result['error'][:100]}...")
                all_available = False

        self.test_results.append({
            'test': 'component_availability',
            'components': component_results,
            'overall_success': all_available
        })

        print()
        return all_available

    def test_monitoring_system(self) -> bool:
        """Test 2: Validate monitoring system functionality"""
        print("🧪 Test 2: Monitoring System")
        print("-" * 40)

        monitor_script = self.scripts_dir / 'ai-cost-monitor.py'

        # Test status check
        status_result = self.run_command(monitor_script, ['status'])
        status_ok = status_result['success'] and 'Error' not in status_result['output']

        # Test logging
        log_result = self.run_command(monitor_script, [
            'log', '--agent', 'TestComponent', '--model', 'claude-3-haiku',
            '--tokens', '150', '--task-type', 'code_review'
        ])
        log_ok = log_result['success']

        # Test stats
        stats_result = self.run_command(monitor_script, ['status'])
        stats_ok = stats_result['success']

        tests = [
            ('Status Check', status_ok),
            ('Usage Logging', log_ok),
            ('Statistics', stats_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'monitoring_system',
            'status_checks': status_result,
            'logging': log_result,
            'statistics': stats_result,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_caching_system(self) -> bool:
        """Test 3: Validate response caching functionality"""
        print("🧪 Test 3: Caching System")
        print("-" * 40)

        cache_script = self.scripts_dir / 'ai-cache.py'

        # Test cache stats
        stats_result = self.run_command(cache_script, ['stats'])
        stats_ok = stats_result['success']

        # Test cache store
        test_prompt = "Write a function to calculate fibonacci numbers"
        test_response = "def fibonacci(n): return n if n <= 1 else fibonacci(n-1) + fibonacci(n-2)"

        store_result = self.run_command(cache_script, [
            'store', '--prompt', test_prompt, '--response', test_response,
            '--model', 'claude-3-haiku', '--tokens', '75', '--cost', '0.0015'
        ])
        store_ok = store_result['success']

        # Test cache retrieval
        get_result = self.run_command(cache_script, ['get', '--prompt', test_prompt])
        get_ok = get_result['success'] and 'Cache hit!' in get_result['output']

        # Test cache cleanup
        cleanup_result = self.run_command(cache_script, ['clear', '--older-than', '0'])
        cleanup_ok = cleanup_result['success']

        tests = [
            ('Cache Stats', stats_ok),
            ('Store Response', store_ok),
            ('Retrieve Response', get_ok),
            ('Cleanup Cache', cleanup_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'caching_system',
            'stats': stats_result,
            'store': store_result,
            'retrieve': get_result,
            'cleanup': cleanup_result,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_quality_system(self) -> bool:
        """Test 4: Validate quality scoring functionality"""
        print("🧪 Test 4: Quality Scoring System")
        print("-" * 40)

        quality_script = self.scripts_dir / 'ai-quality-scorer.py'

        # Test quality stats
        stats_result = self.run_command(quality_script, ['stats'])
        stats_ok = stats_result['success']

        # Test quality evaluation
        test_prompt = "Explain how recursion works"
        test_response = "Recursion is a programming technique where a function calls itself to solve a problem by breaking it down into smaller, similar subproblems."

        eval_result = self.run_command(quality_script, [
            'evaluate', '--prompt', test_prompt, '--response', test_response,
            '--task-type', 'documentation'
        ])
        eval_ok = eval_result['success']

        tests = [
            ('Quality Stats', stats_ok),
            ('Quality Evaluation', eval_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'quality_system',
            'stats': stats_result,
            'evaluation': eval_result,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_batch_processing(self) -> bool:
        """Test 5: Validate batch processing functionality"""
        print("🧪 Test 5: Batch Processing System")
        print("-" * 40)

        batch_script = self.scripts_dir / 'ai-batch-processor.py'

        # Test batch stats
        stats_result = self.run_command(batch_script, ['stats'])
        stats_ok = stats_result['success']

        # Test adding requests to batch
        add_result = self.run_command(batch_script, [
            'submit', '--prompt', 'Write a hello world function',
            '--priority', '2'
        ])
        add_ok = add_result['success']

        # Test batch status command availability (don't check actual status since no requests)
        process_result = self.run_command(batch_script, ['status', '--help'])
        process_ok = process_result['success'] and 'request-id' in process_result['output']

        tests = [
            ('Batch Stats', stats_ok),
            ('Add to Batch', add_ok),
            ('Process Batch', process_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'batch_processing',
            'stats': stats_result,
            'add_request': add_result,
            'process_batch': process_result,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_model_selection(self) -> bool:
        """Test 6: Validate model selection functionality"""
        print("🧪 Test 6: Model Selection System")
        print("-" * 40)

        selector_script = self.scripts_dir / 'ai-model-selector.py'

        # Test model selection for different tasks
        tasks = ['code_review', 'debugging', 'documentation', 'analysis']

        task_results = []
        all_pass = True
        for task in tasks:
            result = self.run_command(selector_script, [task, '--complexity', 'medium'])
            passed = result['success'] and 'Error' not in result['output']

            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {task} selection")

            task_results.append({
                'task': task,
                'success': passed,
                'error': result['error'][:100] if not result['success'] else None
            })

            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'model_selection',
            'tasks': task_results,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_integration_flow(self) -> bool:
        """Test 7: End-to-end integration flow"""
        print("🧪 Test 7: Integration Flow")
        print("-" * 40)

        center_script = self.scripts_dir / 'ai-optimization-center.py'

        # Test system status
        status_result = self.run_command(center_script, ['status'])
        status_ok = status_result['success']

        # Test request optimization
        optimize_result = self.run_command(center_script, [
            'optimize', '--prompt', 'Create a Python function to reverse a string',
            '--task-type', 'code'
        ])
        optimize_ok = optimize_result['success']

        # Test request processing
        process_result = self.run_command(center_script, [
            'process', '--prompt', 'Create a Python function to reverse a string',
            '--task-type', 'code'
        ])
        process_ok = process_result['success']

        # Test optimization report
        report_result = self.run_command(center_script, ['report'])
        report_ok = report_result['success']

        tests = [
            ('System Status', status_ok),
            ('Request Optimization', optimize_ok),
            ('Request Processing', process_ok),
            ('Optimization Report', report_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'integration_flow',
            'status': status_result,
            'optimize': optimize_result,
            'process': process_result,
            'report': report_result,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def test_performance_simulation(self) -> bool:
        """Test 8: Performance simulation with realistic usage patterns"""
        print("🧪 Test 8: Performance Simulation")
        print("-" * 40)

        # Simulate realistic usage patterns
        test_prompts = [
            "Write a function to validate email addresses",
            "Debug this JavaScript error: TypeError: Cannot read property",
            "Create unit tests for a user authentication service",
            "Document the API endpoints for user management",
            "Optimize this SQL query for better performance"
        ]

        center_script = self.scripts_dir / 'ai-optimization-center.py'
        monitor_script = self.scripts_dir / 'ai-cost-monitor.py'

        total_requests = 0
        cached_requests = 0
        total_cost = 0.0

        print("  Simulating 25 AI requests with optimization...")

        for i, prompt in enumerate(test_prompts * 5):  # 5 rounds
            total_requests += 1

            # Process through optimization center
            result = self.run_command(center_script, [
                'process', '--prompt', prompt, '--task-type', 'code'
            ])

            if result['success']:
                # Check if it was cached (simplified check)
                if 'cache_hit' in result['output'] or 'cache' in result['output'].lower():
                    cached_requests += 1

                # Extract cost from output (simplified)
                if '$' in result['output']:
                    try:
                        cost_line = [line for line in result['output'].split('\n') if '$' in line][0]
                        cost = float(cost_line.split('$')[1].split()[0])
                        total_cost += cost
                    except:
                        pass

        # Get final stats
        stats_result = self.run_command(monitor_script, ['stats'])

        cache_ratio = cached_requests / total_requests if total_requests > 0 else 0
        avg_cost_per_request = total_cost / total_requests if total_requests > 0 else 0

        print(f"  Total Requests: {total_requests}")
        print(f"  Cached Requests: {cached_requests}")
        print(f"  Cache Hit Rate: {cache_ratio:.1%}")
        print(f"  Total Cost: ${total_cost:.4f}")
        print(f"  Avg Cost/Request: ${avg_cost_per_request:.4f}")

        # Performance criteria
        cache_ok = cache_ratio >= 0.1  # At least 10% cache hit rate
        cost_ok = avg_cost_per_request <= 0.01  # Max $0.01 per request
        volume_ok = total_requests == 25  # All requests processed

        tests = [
            ('Request Volume', volume_ok),
            ('Cache Performance', cache_ok),
            ('Cost Efficiency', cost_ok)
        ]

        all_pass = True
        for test_name, passed in tests:
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"  {status} {test_name}")
            if not passed:
                all_pass = False

        self.test_results.append({
            'test': 'performance_simulation',
            'total_requests': total_requests,
            'cached_requests': cached_requests,
            'cache_hit_rate': cache_ratio,
            'total_cost': total_cost,
            'avg_cost_per_request': avg_cost_per_request,
            'overall_success': all_pass
        })

        print()
        return all_pass

    def generate_report(self) -> Dict[str, Any]:
        """Generate comprehensive test report"""
        end_time = datetime.now()
        duration = (end_time - self.start_time).total_seconds()

        # Count results
        total_tests = len(self.test_results)
        passed_tests = sum(1 for test in self.test_results if test.get('overall_success', False))

        # Calculate scores by category
        categories = {}
        for test in self.test_results:
            category = test['test']
            if category not in categories:
                categories[category] = {'passed': 0, 'total': 0}
            categories[category]['total'] += 1
            if test.get('overall_success', False):
                categories[category]['passed'] += 1

        report = {
            'test_run': {
                'start_time': self.start_time.isoformat(),
                'end_time': end_time.isoformat(),
                'duration_seconds': duration,
                'total_tests': total_tests,
                'passed_tests': passed_tests,
                'success_rate': passed_tests / total_tests if total_tests > 0 else 0
            },
            'categories': categories,
            'detailed_results': self.test_results,
            'recommendations': []
        }

        # Generate recommendations
        if passed_tests < total_tests:
            report['recommendations'].append("Fix failing components before production use")

        if report['test_run']['success_rate'] >= 0.8:
            report['recommendations'].append("System ready for production with monitoring")
        else:
            report['recommendations'].append("Additional testing and fixes required")

        return report

    def run_all_tests(self) -> bool:
        """Run complete test suite"""
        print("🚀 AI Optimization Integration Test Suite")
        print("=" * 60)
        print(f"Started: {self.start_time.strftime('%Y-%m-%d %H:%M:%S')}")
        print()

        tests = [
            self.test_component_availability,
            self.test_monitoring_system,
            self.test_caching_system,
            self.test_quality_system,
            self.test_batch_processing,
            self.test_model_selection,
            self.test_integration_flow,
            self.test_performance_simulation
        ]

        overall_success = True
        for test_func in tests:
            try:
                success = test_func()
                if not success:
                    overall_success = False
            except Exception as e:
                print(f"❌ Test failed with exception: {e}")
                overall_success = False
                self.test_results.append({
                    'test': test_func.__name__,
                    'exception': str(e),
                    'overall_success': False
                })

        # Generate and display report
        report = self.generate_report()

        print("📊 Test Results Summary")
        print("=" * 60)
        run_info = report['test_run']
        print(f"Duration: {run_info['duration_seconds']:.1f} seconds")
        print(f"Tests Run: {run_info['total_tests']}")
        print(f"Tests Passed: {run_info['passed_tests']}")
        print(f"Success Rate: {run_info['success_rate']:.1%}")
        print()

        print("📈 Category Results:")
        for category, stats in report['categories'].items():
            rate = stats['passed'] / stats['total'] if stats['total'] > 0 else 0
            status = "✅" if rate == 1.0 else "⚠️" if rate >= 0.5 else "❌"
            print(f"  {status} {category.replace('_', ' ').title()}: {stats['passed']}/{stats['total']} ({rate:.1%})")
        print()

        if report['recommendations']:
            print("💡 Recommendations:")
            for rec in report['recommendations']:
                print(f"  • {rec}")

        print()
        final_status = "🎉 ALL TESTS PASSED" if overall_success else "⚠️  SOME TESTS FAILED"
        print(final_status)

        return overall_success

def main():
    project_root = Path(__file__).parent.parent
    tester = AIOptimizationTester(project_root)

    success = tester.run_all_tests()

    # Save detailed results to file
    results_file = project_root / 'logs' / 'ai-integration-test-results.json'
    results_file.parent.mkdir(exist_ok=True)

    with open(results_file, 'w') as f:
        json.dump(tester.generate_report(), f, indent=2, default=str)

    print(f"\n📄 Detailed results saved to: {results_file}")

    sys.exit(0 if success else 1)

if __name__ == '__main__':
    main()