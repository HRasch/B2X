#!/usr/bin/env python3
"""
AI Batch Processing System
Groups similar AI requests for efficient processing and cost optimization
Part of the Agent Escalation System for cost-effective AI usage
"""

import json
import time
import threading
from pathlib import Path
from typing import Dict, List, Optional, Any, Callable, Tuple
from dataclasses import dataclass, field
from datetime import datetime, timedelta
from queue import Queue, Empty
import argparse
import sys

@dataclass
class BatchRequest:
    """Represents a single AI request in a batch"""
    id: str
    prompt: str
    context: Optional[str] = None
    priority: int = 1  # 1=low, 2=medium, 3=high
    max_tokens: int = 1000
    temperature: float = 0.7
    submitted_at: datetime = field(default_factory=datetime.now)
    metadata: Dict[str, Any] = field(default_factory=dict)

@dataclass
class BatchGroup:
    """Represents a group of similar requests"""
    id: str
    requests: List[BatchRequest] = field(default_factory=list)
    common_prompt: str = ""
    combined_context: str = ""
    estimated_tokens: int = 0
    priority: int = 1
    created_at: datetime = field(default_factory=datetime.now)

@dataclass
class BatchResult:
    """Result of processing a batch"""
    batch_id: str
    request_results: Dict[str, Dict[str, Any]] = field(default_factory=dict)
    total_tokens: int = 0
    total_cost: float = 0.0
    processing_time: float = 0.0
    completed_at: Optional[datetime] = None

class BatchProcessor:
    """Intelligent batch processing system for AI requests"""

    def __init__(self, project_root: Path, max_batch_size: int = 5, max_wait_time: int = 30):
        self.project_root = project_root
        self.max_batch_size = max_batch_size
        self.max_wait_time = max_wait_time  # seconds

        # Thread safety
        self.lock = threading.Lock()

        # Queues and storage
        self.request_queue: Queue = Queue()
        self.batch_groups: Dict[str, BatchGroup] = {}
        self.processing_results: Dict[str, BatchResult] = {}

        # Processing control
        self.is_running = False
        self.processing_thread: Optional[threading.Thread] = None

        # Callbacks
        self.on_batch_ready: Optional[Callable[[BatchGroup], None]] = None
        self.on_batch_complete: Optional[Callable[[BatchResult], None]] = None

        # Load existing state
        self._load_state()

    def _load_state(self):
        """Load batch processing state from disk"""
        state_file = self.project_root / '.ai' / 'cache' / 'batch-state.json'
        if state_file.exists():
            try:
                with open(state_file, 'r') as f:
                    data = json.load(f)

                # Restore pending requests
                with self.lock:
                    for request_data in data.get('pending_requests', []):
                        request_data['submitted_at'] = datetime.fromisoformat(request_data['submitted_at'])
                        request = BatchRequest(**request_data)
                        self.request_queue.put(request)

                # Restore batch groups
                with self.lock:
                    for group_data in data.get('batch_groups', []):
                        group_data['created_at'] = datetime.fromisoformat(group_data['created_at'])
                        # Convert request dicts back to BatchRequest objects
                        requests = []
                        for req_data in group_data['requests']:
                            req_data['submitted_at'] = datetime.fromisoformat(req_data['submitted_at'])
                            requests.append(BatchRequest(**req_data))
                        group_data['requests'] = requests
                        group = BatchGroup(**group_data)
                        self.batch_groups[group.id] = group

            except Exception as e:
                print(f"Warning: Could not load batch state: {e}", file=sys.stderr)

    def _save_state(self):
        """Save batch processing state to disk"""
        state_file = self.project_root / '.ai' / 'cache' / 'batch-state.json'
        state_file.parent.mkdir(parents=True, exist_ok=True)

        with self.lock:
            try:
                # Convert objects to serializable format
                pending_requests = []
                while not self.request_queue.empty():
                    try:
                        request = self.request_queue.get_nowait()
                        request_data = {
                            'id': request.id,
                            'prompt': request.prompt,
                            'context': request.context,
                            'priority': request.priority,
                            'max_tokens': request.max_tokens,
                            'temperature': request.temperature,
                            'submitted_at': request.submitted_at.isoformat(),
                            'metadata': request.metadata
                        }
                        pending_requests.append(request_data)
                    except Empty:
                        break

                # Put requests back in queue
                for req in pending_requests:
                    req['submitted_at'] = datetime.fromisoformat(req['submitted_at'])
                    self.request_queue.put(BatchRequest(**req))

                batch_groups = []
                for group in self.batch_groups.values():
                    group_data = {
                        'id': group.id,
                        'requests': [{
                            'id': r.id,
                            'prompt': r.prompt,
                            'context': r.context,
                            'priority': r.priority,
                            'max_tokens': r.max_tokens,
                            'temperature': r.temperature,
                            'submitted_at': r.submitted_at.isoformat(),
                            'metadata': r.metadata
                        } for r in group.requests],
                        'common_prompt': group.common_prompt,
                        'combined_context': group.combined_context,
                        'estimated_tokens': group.estimated_tokens,
                        'priority': group.priority,
                        'created_at': group.created_at.isoformat()
                    }
                    batch_groups.append(group_data)

                state = {
                    'pending_requests': pending_requests,
                    'batch_groups': batch_groups,
                    'saved_at': datetime.now().isoformat()
                }

                with open(state_file, 'w') as f:
                    json.dump(state, f, indent=2)

            except Exception as e:
                print(f"Error saving batch state: {e}", file=sys.stderr)

    def submit_request(self, prompt: str, context: Optional[str] = None,
                      priority: int = 1, max_tokens: int = 1000,
                      temperature: float = 0.7, metadata: Optional[Dict[str, Any]] = None) -> str:
        """
        Submit a request for batch processing

        Returns request ID
        """
        request_id = f"req_{int(time.time() * 1000)}_{hash(prompt) % 10000:04d}"

        request = BatchRequest(
            id=request_id,
            prompt=prompt,
            context=context,
            priority=priority,
            max_tokens=max_tokens,
            temperature=temperature,
            metadata=metadata or {}
        )

        with self.lock:
            self.request_queue.put(request)
            self._save_state()

        return request_id

    def get_request_status(self, request_id: str) -> Dict[str, Any]:
        """Get the status of a submitted request"""
        # Check if request is still in queue
        queue_requests = []
        found_request = None

        while not self.request_queue.empty():
            try:
                req = self.request_queue.get_nowait()
                if req.id == request_id:
                    found_request = req
                else:
                    queue_requests.append(req)
            except Empty:
                break

        # Put requests back
        for req in queue_requests:
            self.request_queue.put(req)
        if found_request:
            self.request_queue.put(found_request)

        if found_request:
            return {
                'status': 'queued',
                'position': len(queue_requests),
                'submitted_at': found_request.submitted_at.isoformat(),
                'priority': found_request.priority
            }

        # Check if request is in a batch group
        for group in self.batch_groups.values():
            for request in group.requests:
                if request.id == request_id:
                    return {
                        'status': 'batching',
                        'batch_id': group.id,
                        'batch_size': len(group.requests),
                        'submitted_at': request.submitted_at.isoformat()
                    }

        # Check if request has been processed
        for result in self.processing_results.values():
            if request_id in result.request_results:
                req_result = result.request_results[request_id]
                return {
                    'status': 'completed',
                    'batch_id': result.batch_id,
                    'completed_at': result.completed_at.isoformat() if result.completed_at else None,
                    'tokens_used': req_result.get('tokens_used', 0),
                    'cost': req_result.get('cost', 0.0),
                    'response': req_result.get('response', '')
                }

        return {'status': 'not_found'}

    def _calculate_similarity(self, prompt1: str, prompt2: str) -> float:
        """Calculate similarity between two prompts"""
        words1 = set(prompt1.lower().split())
        words2 = set(prompt2.lower().split())

        if not words1 or not words2:
            return 0.0

        intersection = words1.intersection(words2)
        union = words1.union(words2)

        return len(intersection) / len(union)

    def _group_similar_requests(self, requests: List[BatchRequest]) -> List[BatchGroup]:
        """Group similar requests for batch processing"""
        groups = []
        processed = set()

        for i, request in enumerate(requests):
            if request.id in processed:
                continue

            group = BatchGroup(
                id=f"batch_{int(time.time() * 1000)}_{i}",
                requests=[request],
                priority=request.priority
            )
            processed.add(request.id)

            # Find similar requests
            for j, other_request in enumerate(requests):
                if other_request.id in processed:
                    continue

                similarity = self._calculate_similarity(request.prompt, other_request.prompt)
                if similarity > 0.6:  # Similarity threshold
                    group.requests.append(other_request)
                    processed.add(other_request.id)

                    # Update group priority to highest in group
                    group.priority = max(group.priority, other_request.priority)

                    # Stop if batch is full
                    if len(group.requests) >= self.max_batch_size:
                        break

            # Create combined prompt and context
            if len(group.requests) > 1:
                group.common_prompt = self._create_batch_prompt(group.requests)
                group.combined_context = self._create_batch_context(group.requests)
                group.estimated_tokens = sum(r.max_tokens for r in group.requests) + 200  # Overhead

            groups.append(group)

        return groups

    def _create_batch_prompt(self, requests: List[BatchRequest]) -> str:
        """Create a combined prompt for batch processing"""
        base_prompt = "Please answer the following related questions efficiently:\n\n"

        for i, request in enumerate(requests, 1):
            base_prompt += f"{i}. {request.prompt}\n"

        base_prompt += "\nProvide concise, accurate answers for each question."
        return base_prompt

    def _create_batch_context(self, requests: List[BatchRequest]) -> str:
        """Create combined context for batch processing"""
        contexts = [r.context for r in requests if r.context]
        if not contexts:
            return ""

        combined = "Shared Context:\n" + "\n".join(contexts[:3])  # Limit to first 3 contexts
        return combined

    def _process_batch(self, batch_group: BatchGroup) -> BatchResult:
        """Process a batch of requests (simulated)"""
        start_time = time.time()

        # Simulate AI processing
        time.sleep(0.5)  # Simulate API call delay

        # Create mock results for each request
        result = BatchResult(batch_id=batch_group.id)
        total_tokens = 0
        total_cost = 0.0

        for request in batch_group.requests:
            # Simulate response generation
            response = f"Response to: {request.prompt[:50]}..."
            tokens_used = min(len(request.prompt.split()) * 2, request.max_tokens)
            cost = tokens_used * 0.00002  # Mock cost calculation

            result.request_results[request.id] = {
                'response': response,
                'tokens_used': tokens_used,
                'cost': cost,
                'model': 'claude-3-haiku',  # Mock model
                'processing_time': time.time() - start_time
            }

            total_tokens += tokens_used
            total_cost += cost

        result.total_tokens = total_tokens
        result.total_cost = total_cost
        result.processing_time = time.time() - start_time
        result.completed_at = datetime.now()

        return result

    def _processing_loop(self):
        """Main processing loop"""
        while self.is_running:
            try:
                # Collect pending requests
                pending_requests = []
                batch_start_time = time.time()

                while len(pending_requests) < self.max_batch_size:
                    try:
                        # Wait for requests with timeout
                        with self.lock:
                            request = self.request_queue.get(timeout=1.0)
                        pending_requests.append(request)

                        # Check if we've waited too long or have enough requests
                        if (time.time() - batch_start_time > self.max_wait_time or
                            len(pending_requests) >= self.max_batch_size):
                            break

                    except Empty:
                        # No more requests, check if we should process what we have
                        if pending_requests and time.time() - batch_start_time > self.max_wait_time:
                            break
                        continue

                if pending_requests:
                    # Group similar requests
                    batch_groups = self._group_similar_requests(pending_requests)

                    with self.lock:
                        for group in batch_groups:
                            self.batch_groups[group.id] = group

                            # Notify batch ready
                            if self.on_batch_ready:
                                self.on_batch_ready(group)

                        # Process batch
                        result = self._process_batch(group)
                        self.processing_results[result.batch_id] = result

                        # Notify batch complete
                        if self.on_batch_complete:
                            self.on_batch_complete(result)

                        # Clean up
                        del self.batch_groups[group.id]

                    self._save_state()

            except Exception as e:
                print(f"Error in processing loop: {e}", file=sys.stderr)
                time.sleep(1.0)

    def start_processing(self):
        """Start the batch processing system"""
        if self.is_running:
            return

        self.is_running = True
        self.processing_thread = threading.Thread(target=self._processing_loop, daemon=True)
        self.processing_thread.start()
        print("✅ Batch processing system started")

    def stop_processing(self):
        """Stop the batch processing system"""
        self.is_running = False
        if self.processing_thread:
            self.processing_thread.join(timeout=5.0)
        self._save_state()
        print("🛑 Batch processing system stopped")

    def get_stats(self) -> Dict[str, Any]:
        """Get batch processing statistics"""
        total_requests = sum(len(group.requests) for group in self.batch_groups.values())
        completed_batches = len(self.processing_results)

        total_tokens = sum(result.total_tokens for result in self.processing_results.values())
        total_cost = sum(result.total_cost for result in self.processing_results.values())
        avg_processing_time = (
            sum(result.processing_time for result in self.processing_results.values()) / completed_batches
            if completed_batches > 0 else 0
        )

        return {
            'is_running': self.is_running,
            'queued_requests': self.request_queue.qsize(),
            'active_batches': len(self.batch_groups),
            'completed_batches': completed_batches,
            'total_requests_processed': sum(len(result.request_results) for result in self.processing_results.values()),
            'total_tokens_processed': total_tokens,
            'total_cost_saved': total_cost * 0.3,  # Estimate 30% savings from batching
            'avg_processing_time': round(avg_processing_time, 2),
            'avg_batch_size': round(total_requests / completed_batches, 1) if completed_batches > 0 else 0
        }

def main():
    parser = argparse.ArgumentParser(description='AI Batch Processing System')
    parser.add_argument('command', choices=['submit', 'status', 'stats', 'start', 'stop'],
                       help='Command to execute')
    parser.add_argument('--request-id', help='Request ID for status check')
    parser.add_argument('--prompt', help='Prompt for request submission')
    parser.add_argument('--priority', type=int, default=1, choices=[1, 2, 3],
                       help='Request priority (1=low, 2=medium, 3=high)')
    parser.add_argument('--max-tokens', type=int, default=1000,
                       help='Maximum tokens for the request')

    args = parser.parse_args()

    # Initialize processor
    project_root = Path(__file__).parent.parent
    processor = BatchProcessor(project_root)

    if args.command == 'submit':
        if not args.prompt:
            print("Error: --prompt required for submission")
            sys.exit(1)

        request_id = processor.submit_request(
            prompt=args.prompt,
            priority=args.priority,
            max_tokens=args.max_tokens
        )
        print(f"✅ Request submitted with ID: {request_id}")

    elif args.command == 'status':
        if not args.request_id:
            print("Error: --request-id required for status check")
            sys.exit(1)

        status = processor.get_request_status(args.request_id)
        print(f"📊 Request Status: {status['status']}")
        if status['status'] == 'queued':
            print(f"Queue position: {status['position']}")
        elif status['status'] == 'batching':
            print(f"Batch ID: {status['batch_id']}")
            print(f"Batch size: {status['batch_size']}")
        elif status['status'] == 'completed':
            print(f"Batch ID: {status['batch_id']}")
            print(f"Tokens used: {status['tokens_used']}")
            print(f"Cost: ${status['cost']:.4f}")
            print(f"Response: {status['response'][:100]}...")

    elif args.command == 'stats':
        stats = processor.get_stats()
        print("📊 Batch Processing Statistics")
        print("=" * 40)
        print(f"System Running: {stats['is_running']}")
        print(f"Queued Requests: {stats['queued_requests']}")
        print(f"Active Batches: {stats['active_batches']}")
        print(f"Completed Batches: {stats['completed_batches']}")
        print(f"Total Requests Processed: {stats['total_requests_processed']}")
        print(f"Total Tokens Processed: {stats['total_tokens_processed']:,}")
        print(f"Estimated Cost Saved: ${stats['total_cost_saved']:.4f}")
        print(f"Average Processing Time: {stats['avg_processing_time']}s")
        print(f"Average Batch Size: {stats['avg_batch_size']}")

    elif args.command == 'start':
        processor.start_processing()
        print("🎯 Batch processing started - use Ctrl+C to stop")

        try:
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            processor.stop_processing()

    elif args.command == 'stop':
        processor.stop_processing()

if __name__ == '__main__':
    main()