#!/usr/bin/env python3
"""
AI Quality Scoring Framework
Evaluates and scores AI responses for quality, relevance, and effectiveness
Part of the Agent Escalation System for continuous improvement
"""

import json
import re
from pathlib import Path
from typing import Dict, List, Optional, Any, Tuple
from dataclasses import dataclass
from datetime import datetime
import argparse
import sys

@dataclass
class QualityMetrics:
    """Quality metrics for AI response evaluation"""
    relevance_score: float  # 0.0-1.0: How relevant to the query
    accuracy_score: float   # 0.0-1.0: Factual correctness
    completeness_score: float  # 0.0-1.0: How complete the answer is
    clarity_score: float    # 0.0-1.0: How clear and understandable
    usefulness_score: float # 0.0-1.0: Practical value to user
    overall_score: float    # 0.0-1.0: Weighted average

    def __post_init__(self):
        self.overall_score = (
            self.relevance_score * 0.25 +
            self.accuracy_score * 0.25 +
            self.completeness_score * 0.2 +
            self.clarity_score * 0.15 +
            self.usefulness_score * 0.15
        )

@dataclass
class QualityFeedback:
    """Detailed feedback for quality improvement"""
    response_id: str
    metrics: QualityMetrics
    strengths: List[str]
    weaknesses: List[str]
    suggestions: List[str]
    evaluated_at: datetime
    evaluator: str

class QualityScorer:
    """AI response quality evaluation system"""

    def __init__(self, project_root: Path):
        self.project_root = project_root
        self.feedback_file = project_root / '.ai' / 'cache' / 'quality-feedback.json'
        self.feedback_file.parent.mkdir(parents=True, exist_ok=True)
        self._load_feedback()

    def _load_feedback(self):
        """Load quality feedback from disk"""
        self.feedback: Dict[str, QualityFeedback] = {}

        if self.feedback_file.exists():
            try:
                with open(self.feedback_file, 'r') as f:
                    data = json.load(f)
                    for key, item in data.items():
                        # Convert datetime string back to datetime object
                        item['evaluated_at'] = datetime.fromisoformat(item['evaluated_at'])
                        # Convert metrics dict back to QualityMetrics
                        item['metrics'] = QualityMetrics(**item['metrics'])
                        self.feedback[key] = QualityFeedback(**item)
            except Exception as e:
                print(f"Warning: Could not load quality feedback: {e}", file=sys.stderr)

    def _save_feedback(self):
        """Save quality feedback to disk"""
        try:
            feedback_data = {}
            for key, feedback in self.feedback.items():
                data = {
                    'response_id': feedback.response_id,
                    'metrics': {
                        'relevance_score': feedback.metrics.relevance_score,
                        'accuracy_score': feedback.metrics.accuracy_score,
                        'completeness_score': feedback.metrics.completeness_score,
                        'clarity_score': feedback.metrics.clarity_score,
                        'usefulness_score': feedback.metrics.usefulness_score,
                        'overall_score': feedback.metrics.overall_score
                    },
                    'strengths': feedback.strengths,
                    'weaknesses': feedback.weaknesses,
                    'suggestions': feedback.suggestions,
                    'evaluated_at': feedback.evaluated_at.isoformat(),
                    'evaluator': feedback.evaluator
                }
                feedback_data[key] = data

            with open(self.feedback_file, 'w') as f:
                json.dump(feedback_data, f, indent=2)

        except Exception as e:
            print(f"Error saving quality feedback: {e}", file=sys.stderr)

    def evaluate_response(self, prompt: str, response: str, task_type: str = "general",
                         evaluator: str = "auto") -> QualityFeedback:
        """
        Evaluate the quality of an AI response

        Returns detailed quality feedback
        """
        response_id = f"{hash(prompt + response) % 1000000:06d}"

        # Perform automated quality assessment
        metrics = self._calculate_metrics(prompt, response, task_type)

        # Generate feedback based on metrics
        strengths, weaknesses, suggestions = self._generate_feedback(prompt, response, metrics, task_type)

        feedback = QualityFeedback(
            response_id=response_id,
            metrics=metrics,
            strengths=strengths,
            weaknesses=weaknesses,
            suggestions=suggestions,
            evaluated_at=datetime.now(),
            evaluator=evaluator
        )

        self.feedback[response_id] = feedback
        self._save_feedback()

        return feedback

    def _calculate_metrics(self, prompt: str, response: str, task_type: str) -> QualityMetrics:
        """Calculate quality metrics for the response"""

        # Relevance: Check if response addresses the prompt
        relevance_score = self._calculate_relevance(prompt, response)

        # Accuracy: Check for factual correctness (basic heuristics)
        accuracy_score = self._calculate_accuracy(response, task_type)

        # Completeness: Check if response is comprehensive
        completeness_score = self._calculate_completeness(prompt, response, task_type)

        # Clarity: Check readability and structure
        clarity_score = self._calculate_clarity(response)

        # Usefulness: Check practical value
        usefulness_score = self._calculate_usefulness(response, task_type)

        return QualityMetrics(
            relevance_score=relevance_score,
            accuracy_score=accuracy_score,
            completeness_score=completeness_score,
            clarity_score=clarity_score,
            usefulness_score=usefulness_score,
            overall_score=0.0  # Will be calculated in __post_init__
        )

    def _calculate_relevance(self, prompt: str, response: str) -> float:
        """Calculate relevance score"""
        prompt_words = set(prompt.lower().split())
        response_words = set(response.lower().split())

        if not prompt_words:
            return 0.5

        overlap = len(prompt_words.intersection(response_words))
        relevance = overlap / len(prompt_words)

        # Boost score if response directly addresses key terms
        key_terms = ['how', 'what', 'why', 'when', 'where', 'which', 'can', 'should', 'would']
        if any(term in prompt.lower() for term in key_terms):
            if any(term in response.lower() for term in key_terms[:5]):  # Address main questions
                relevance += 0.2

        return min(1.0, relevance)

    def _calculate_accuracy(self, response: str, task_type: str) -> float:
        """Calculate accuracy score using heuristics"""
        score = 0.8  # Base score

        # Penalize for uncertainty markers
        uncertainty_markers = ['i think', 'maybe', 'perhaps', 'probably', 'might', 'could be']
        uncertainty_count = sum(1 for marker in uncertainty_markers if marker in response.lower())
        score -= uncertainty_count * 0.1

        # Boost for confidence markers
        confidence_markers = ['definitely', 'certainly', 'clearly', 'obviously', 'specifically']
        confidence_count = sum(1 for marker in confidence_markers if marker in response.lower())
        score += confidence_count * 0.05

        # Task-specific accuracy checks
        if task_type == 'code':
            # Check for code-like elements
            if '```' in response or 'function' in response or 'class' in response:
                score += 0.1
        elif task_type == 'documentation':
            # Check for structured content
            if any(marker in response for marker in ['##', '- ', '1. ', '2. ']):
                score += 0.1

        return max(0.0, min(1.0, score))

    def _calculate_completeness(self, prompt: str, response: str, task_type: str) -> float:
        """Calculate completeness score"""
        base_score = 0.7

        # Length appropriateness
        word_count = len(response.split())
        if task_type in ['code', 'debugging'] and word_count < 50:
            base_score -= 0.2  # Code responses should be detailed
        elif task_type == 'general' and word_count > 500:
            base_score += 0.1  # General responses can be comprehensive

        # Structure check
        if '\n\n' in response or '\n-' in response:
            base_score += 0.1  # Structured responses are more complete

        # Check for examples
        if 'example' in response.lower() or '```' in response:
            base_score += 0.1

        return max(0.0, min(1.0, base_score))

    def _calculate_clarity(self, response: str) -> float:
        """Calculate clarity score"""
        score = 0.8

        # Check sentence structure
        sentences = re.split(r'[.!?]+', response)
        avg_sentence_length = sum(len(s.split()) for s in sentences) / len(sentences) if sentences else 0

        if avg_sentence_length > 30:
            score -= 0.2  # Very long sentences reduce clarity
        elif avg_sentence_length < 5:
            score -= 0.1  # Very short sentences might be too choppy

        # Check for jargon without explanation
        technical_terms = ['api', 'json', 'http', 'database', 'algorithm']
        unexplained_terms = sum(1 for term in technical_terms if term in response.lower() and f"{term} is" not in response.lower())
        score -= unexplained_terms * 0.05

        # Check for formatting
        if '```' in response or '**' in response or '*' in response:
            score += 0.1  # Good formatting improves clarity

        return max(0.0, min(1.0, score))

    def _calculate_usefulness(self, response: str, task_type: str) -> float:
        """Calculate usefulness score"""
        score = 0.7

        # Check for actionable content
        actionable_indicators = ['try this', 'use this', 'implement', 'run this', 'do this']
        if any(indicator in response.lower() for indicator in actionable_indicators):
            score += 0.2

        # Check for next steps
        if 'next' in response.lower() or 'then' in response.lower():
            score += 0.1

        # Task-specific usefulness
        if task_type == 'code':
            if 'function' in response or 'class' in response or 'import' in response:
                score += 0.1
        elif task_type == 'debugging':
            if 'error' in response.lower() or 'fix' in response.lower():
                score += 0.1

        return max(0.0, min(1.0, score))

    def _generate_feedback(self, prompt: str, response: str, metrics: QualityMetrics,
                          task_type: str) -> Tuple[List[str], List[str], List[str]]:
        """Generate detailed feedback based on metrics"""

        strengths = []
        weaknesses = []
        suggestions = []

        # Relevance feedback
        if metrics.relevance_score > 0.8:
            strengths.append("Highly relevant to the query")
        elif metrics.relevance_score < 0.5:
            weaknesses.append("May not fully address the original question")
            suggestions.append("Ensure response directly answers the specific question asked")

        # Accuracy feedback
        if metrics.accuracy_score > 0.8:
            strengths.append("Appears factually accurate")
        elif metrics.accuracy_score < 0.6:
            weaknesses.append("May contain uncertain or speculative information")
            suggestions.append("Use more definitive language and verify facts")

        # Completeness feedback
        if metrics.completeness_score > 0.8:
            strengths.append("Comprehensive and thorough")
        elif metrics.completeness_score < 0.6:
            weaknesses.append("Response seems incomplete")
            suggestions.append("Provide more detailed examples or additional context")

        # Clarity feedback
        if metrics.clarity_score > 0.8:
            strengths.append("Clear and well-structured")
        elif metrics.clarity_score < 0.6:
            weaknesses.append("Could be clearer or better organized")
            suggestions.append("Use shorter sentences and better formatting")

        # Usefulness feedback
        if metrics.usefulness_score > 0.8:
            strengths.append("Highly practical and actionable")
        elif metrics.usefulness_score < 0.6:
            weaknesses.append("May lack practical guidance")
            suggestions.append("Include specific steps or actionable recommendations")

        # Task-specific feedback
        if task_type == 'code':
            if '```' in response:
                strengths.append("Includes code examples")
            else:
                suggestions.append("Add code snippets to illustrate solutions")
        elif task_type == 'debugging':
            if 'error' in response.lower():
                strengths.append("Addresses potential errors")
            else:
                suggestions.append("Consider common error scenarios")

        return strengths, weaknesses, suggestions

    def get_quality_stats(self) -> Dict[str, Any]:
        """Get quality statistics across all evaluations"""
        if not self.feedback:
            return {
                'total_evaluations': 0,
                'avg_overall_score': 0.0,
                'avg_relevance': 0.0,
                'avg_accuracy': 0.0,
                'avg_completeness': 0.0,
                'avg_clarity': 0.0,
                'avg_usefulness': 0.0,
                'common_strengths': [],
                'common_weaknesses': []
            }

        evaluations = list(self.feedback.values())
        metrics = [e.metrics for e in evaluations]

        # Calculate averages
        avg_scores = {
            'overall_score': sum(m.overall_score for m in metrics) / len(metrics),
            'relevance_score': sum(m.relevance_score for m in metrics) / len(metrics),
            'accuracy_score': sum(m.accuracy_score for m in metrics) / len(metrics),
            'completeness_score': sum(m.completeness_score for m in metrics) / len(metrics),
            'clarity_score': sum(m.clarity_score for m in metrics) / len(metrics),
            'usefulness_score': sum(m.usefulness_score for m in metrics) / len(metrics)
        }

        # Find common strengths and weaknesses
        all_strengths = [item for e in evaluations for item in e.strengths]
        all_weaknesses = [item for e in evaluations for item in e.weaknesses]

        common_strengths = self._get_most_common(all_strengths, 3)
        common_weaknesses = self._get_most_common(all_weaknesses, 3)

        return {
            'total_evaluations': len(evaluations),
            'avg_overall_score': round(avg_scores['overall_score'], 3),
            'avg_relevance': round(avg_scores['relevance_score'], 3),
            'avg_accuracy': round(avg_scores['accuracy_score'], 3),
            'avg_completeness': round(avg_scores['completeness_score'], 3),
            'avg_clarity': round(avg_scores['clarity_score'], 3),
            'avg_usefulness': round(avg_scores['usefulness_score'], 3),
            'common_strengths': common_strengths,
            'common_weaknesses': common_weaknesses
        }

    def _get_most_common(self, items: List[str], n: int) -> List[str]:
        """Get the most common items from a list"""
        if not items:
            return []

        from collections import Counter
        return [item for item, _ in Counter(items).most_common(n)]

def main():
    parser = argparse.ArgumentParser(description='AI Quality Scoring Framework')
    parser.add_argument('command', choices=['evaluate', 'stats', 'feedback'],
                       help='Command to execute')
    parser.add_argument('--prompt', help='User prompt for evaluation')
    parser.add_argument('--response', help='AI response to evaluate')
    parser.add_argument('--task-type', default='general',
                       choices=['general', 'code', 'debugging', 'documentation', 'testing'],
                       help='Type of task')
    parser.add_argument('--evaluator', default='auto', help='Evaluator name')

    args = parser.parse_args()

    # Initialize scorer
    project_root = Path(__file__).parent.parent
    scorer = QualityScorer(project_root)

    if args.command == 'evaluate':
        if not args.prompt or not args.response:
            print("Error: --prompt and --response required for evaluation")
            sys.exit(1)

        feedback = scorer.evaluate_response(
            prompt=args.prompt,
            response=args.response,
            task_type=args.task_type,
            evaluator=args.evaluator
        )

        print("🎯 AI Response Quality Evaluation")
        print("=" * 40)
        print(f"Response ID: {feedback.response_id}")
        print(f"Overall Score: {feedback.metrics.overall_score:.3f}")
        print()
        print("📊 Detailed Metrics:")
        print(f"  Relevance: {feedback.metrics.relevance_score:.3f}")
        print(f"  Accuracy: {feedback.metrics.accuracy_score:.3f}")
        print(f"  Completeness: {feedback.metrics.completeness_score:.3f}")
        print(f"  Clarity: {feedback.metrics.clarity_score:.3f}")
        print(f"  Usefulness: {feedback.metrics.usefulness_score:.3f}")
        print()
        print("💪 Strengths:")
        for strength in feedback.strengths:
            print(f"  ✓ {strength}")
        print()
        print("⚠️  Weaknesses:")
        for weakness in feedback.weaknesses:
            print(f"  ✗ {weakness}")
        print()
        print("💡 Suggestions:")
        for suggestion in feedback.suggestions:
            print(f"  → {suggestion}")

    elif args.command == 'stats':
        stats = scorer.get_quality_stats()
        print("📊 AI Quality Statistics")
        print("=" * 40)
        print(f"Total Evaluations: {stats['total_evaluations']}")
        print(f"Average Overall Score: {stats['avg_overall_score']:.3f}")
        print()
        print("📈 Average Metrics:")
        print(f"  Relevance: {stats['avg_relevance']:.3f}")
        print(f"  Accuracy: {stats['avg_accuracy']:.3f}")
        print(f"  Completeness: {stats['avg_completeness']:.3f}")
        print(f"  Clarity: {stats['avg_clarity']:.3f}")
        print(f"  Usefulness: {stats['avg_usefulness']:.3f}")
        print()
        print("💪 Common Strengths:")
        for strength in stats['common_strengths']:
            print(f"  ✓ {strength}")
        print()
        print("⚠️  Common Weaknesses:")
        for weakness in stats['common_weaknesses']:
            print(f"  ✗ {weakness}")

    elif args.command == 'feedback':
        if not scorer.feedback:
            print("No quality feedback available")
            return

        print("📋 Recent Quality Feedback")
        print("=" * 40)
        for i, (response_id, feedback) in enumerate(list(scorer.feedback.items())[-5:]):  # Last 5
            print(f"{i+1}. Response {response_id}: Score {feedback.metrics.overall_score:.3f}")
            if feedback.strengths:
                print(f"   ✓ {feedback.strengths[0]}")
            if feedback.weaknesses:
                print(f"   ✗ {feedback.weaknesses[0]}")
            print()

if __name__ == '__main__':
    main()