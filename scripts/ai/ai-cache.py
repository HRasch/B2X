#!/usr/bin/env python3
"""
AI Response Caching System
Caches AI responses to reduce redundant API calls and costs
Part of the Agent Escalation System for cost-effective AI usage
"""

import json
import hashlib
import time
from pathlib import Path
from typing import Dict, List, Optional, Any, Tuple
from dataclasses import dataclass, asdict
from datetime import datetime, timedelta
import argparse
import sys

@dataclass
class CachedResponse:
    """Represents a cached AI response"""
    prompt_hash: str
    prompt: str
    response: str
    model: str
    tokens_used: int
    cost: float
    quality_score: Optional[float]
    created_at: datetime
    last_accessed: datetime
    access_count: int
    context_hash: Optional[str] = None
    metadata: Optional[Dict[str, Any]] = None

class AICache:
    """Intelligent AI response caching system"""

    def __init__(self, project_root: Path, cache_dir: Optional[Path] = None):
        self.project_root = project_root
        self.cache_dir = cache_dir or project_root / '.ai' / 'cache'
        self.cache_file = self.cache_dir / 'responses.json'
        self.index_file = self.cache_dir / 'index.json'

        self.cache_dir.mkdir(parents=True, exist_ok=True)
        self._load_cache()

        # Cache configuration
        self.max_cache_size = 1000  # Maximum number of cached responses
        self.max_age_days = 30  # Maximum age of cached responses
        self.similarity_threshold = 0.85  # Minimum similarity for cache hits
        self.auto_cleanup_enabled = True

    def _load_cache(self):
        """Load cache from disk"""
        self.cache: Dict[str, CachedResponse] = {}
        self.index: Dict[str, List[str]] = {}  # prompt_hash -> [context_hashes]

        if self.cache_file.exists():
            try:
                with open(self.cache_file, 'r') as f:
                    data = json.load(f)
                    for key, item in data.items():
                        # Convert datetime strings back to datetime objects
                        item['created_at'] = datetime.fromisoformat(item['created_at'])
                        item['last_accessed'] = datetime.fromisoformat(item['last_accessed'])
                        self.cache[key] = CachedResponse(**item)
            except Exception as e:
                print(f"Warning: Could not load cache: {e}", file=sys.stderr)

        if self.index_file.exists():
            try:
                with open(self.index_file, 'r') as f:
                    self.index = json.load(f)
            except Exception as e:
                print(f"Warning: Could not load index: {e}", file=sys.stderr)

    def _save_cache(self):
        """Save cache to disk"""
        try:
            # Convert datetime objects to ISO strings for JSON serialization
            cache_data = {}
            for key, response in self.cache.items():
                data = asdict(response)
                data['created_at'] = response.created_at.isoformat()
                data['last_accessed'] = response.last_accessed.isoformat()
                cache_data[key] = data

            with open(self.cache_file, 'w') as f:
                json.dump(cache_data, f, indent=2)

            with open(self.index_file, 'w') as f:
                json.dump(self.index, f, indent=2)

        except Exception as e:
            print(f"Error saving cache: {e}", file=sys.stderr)

    def _generate_prompt_hash(self, prompt: str, context: Optional[str] = None) -> str:
        """Generate a hash for the prompt and optional context"""
        content = prompt
        if context:
            content += f"\n---CONTEXT---\n{context}"
        return hashlib.sha256(content.encode()).hexdigest()[:16]

    def _calculate_similarity(self, prompt1: str, prompt2: str) -> float:
        """Calculate similarity between two prompts (simple implementation)"""
        # This is a basic implementation - could be enhanced with NLP
        words1 = set(prompt1.lower().split())
        words2 = set(prompt2.lower().split())

        if not words1 or not words2:
            return 0.0

        intersection = words1.intersection(words2)
        union = words1.union(words2)

        return len(intersection) / len(union)

    def _cleanup_cache(self):
        """Clean up old and low-quality cache entries"""
        if not self.auto_cleanup_enabled:
            return

        current_time = datetime.now()
        to_remove = []

        for key, response in self.cache.items():
            # Remove entries older than max_age_days
            if (current_time - response.created_at).days > self.max_age_days:
                to_remove.append(key)
                continue

            # Remove entries with very low quality scores
            if response.quality_score is not None and response.quality_score < 0.3:
                to_remove.append(key)
                continue

            # Remove least accessed entries if cache is too large
            if len(self.cache) > self.max_cache_size:
                # Sort by access count and recency, remove oldest/least used
                sorted_entries = sorted(
                    self.cache.items(),
                    key=lambda x: (x[1].access_count, x[1].last_accessed.timestamp())
                )
                to_remove.extend([k for k, _ in sorted_entries[:len(sorted_entries) - self.max_cache_size]])

        for key in set(to_remove):  # Remove duplicates
            if key in self.cache:
                del self.cache[key]
                # Clean up index
                if key in self.index:
                    del self.index[key]

        if to_remove:
            print(f"Cache cleanup: removed {len(set(to_remove))} entries", file=sys.stderr)
            self._save_cache()

    def store_response(self, prompt: str, response: str, model: str,
                      tokens_used: int, cost: float,
                      quality_score: Optional[float] = None,
                      context: Optional[str] = None,
                      metadata: Optional[Dict[str, Any]] = None) -> str:
        """
        Store an AI response in the cache

        Returns the cache key
        """
        prompt_hash = self._generate_prompt_hash(prompt, context)
        context_hash = self._generate_prompt_hash(context) if context else None

        cached_response = CachedResponse(
            prompt_hash=prompt_hash,
            prompt=prompt,
            response=response,
            model=model,
            tokens_used=tokens_used,
            cost=cost,
            quality_score=quality_score,
            created_at=datetime.now(),
            last_accessed=datetime.now(),
            access_count=1,
            context_hash=context_hash,
            metadata=metadata or {}
        )

        self.cache[prompt_hash] = cached_response

        # Update index
        if context_hash:
            if prompt_hash not in self.index:
                self.index[prompt_hash] = []
            if context_hash not in self.index[prompt_hash]:
                self.index[prompt_hash].append(context_hash)

        self._cleanup_cache()
        self._save_cache()

        return prompt_hash

    def get_cached_response(self, prompt: str, context: Optional[str] = None,
                           min_quality: Optional[float] = None) -> Optional[CachedResponse]:
        """
        Retrieve a cached response for the given prompt

        Returns None if no suitable cached response is found
        """
        prompt_hash = self._generate_prompt_hash(prompt, context)

        # Direct hash match
        if prompt_hash in self.cache:
            cached = self.cache[prompt_hash]
            if min_quality is None or (cached.quality_score and cached.quality_score >= min_quality):
                cached.last_accessed = datetime.now()
                cached.access_count += 1
                self._save_cache()
                return cached

        # Similarity-based search
        if self.similarity_threshold > 0:
            best_match = None
            best_similarity = 0.0

            for cached_response in self.cache.values():
                similarity = self._calculate_similarity(prompt, cached_response.prompt)

                if similarity >= self.similarity_threshold and similarity > best_similarity:
                    if min_quality is None or (cached_response.quality_score and cached_response.quality_score >= min_quality):
                        best_match = cached_response
                        best_similarity = similarity

            if best_match:
                best_match.last_accessed = datetime.now()
                best_match.access_count += 1
                self._save_cache()
                return best_match

        return None

    def get_cache_stats(self) -> Dict[str, Any]:
        """Get comprehensive cache statistics"""
        if not self.cache:
            return {
                'total_entries': 0,
                'total_cost_saved': 0.0,
                'total_tokens_saved': 0,
                'cache_hit_rate': 0.0,
                'avg_quality_score': 0.0,
                'oldest_entry': None,
                'newest_entry': None
            }

        total_cost = sum(r.cost for r in self.cache.values())
        total_tokens = sum(r.tokens_used for r in self.cache.values())
        total_accesses = sum(r.access_count for r in self.cache.values())

        quality_scores = [r.quality_score for r in self.cache.values() if r.quality_score is not None]
        avg_quality = sum(quality_scores) / len(quality_scores) if quality_scores else 0.0

        created_dates = [r.created_at for r in self.cache.values()]
        oldest = min(created_dates) if created_dates else None
        newest = max(created_dates) if created_dates else None

        # Calculate potential savings (assuming 50% hit rate)
        estimated_cache_hits = len(self.cache) * 0.5
        estimated_cost_saved = total_cost * 0.5
        estimated_tokens_saved = int(total_tokens * 0.5)

        return {
            'total_entries': len(self.cache),
            'total_cost_saved': round(estimated_cost_saved, 4),
            'total_tokens_saved': estimated_tokens_saved,
            'cache_hit_rate': round(estimated_cache_hits / max(total_accesses, 1), 3),
            'avg_quality_score': round(avg_quality, 3),
            'oldest_entry': oldest.isoformat() if oldest else None,
            'newest_entry': newest.isoformat() if newest else None,
            'models_used': list(set(r.model for r in self.cache.values())),
            'total_accesses': total_accesses
        }

    def clear_cache(self, older_than_days: Optional[int] = None):
        """Clear cache entries, optionally only those older than specified days"""
        if older_than_days is None:
            self.cache.clear()
            self.index.clear()
        else:
            cutoff_date = datetime.now() - timedelta(days=older_than_days)
            to_remove = [k for k, v in self.cache.items() if v.created_at < cutoff_date]
            for key in to_remove:
                del self.cache[key]
                if key in self.index:
                    del self.index[key]

        self._save_cache()
        print(f"Cleared {len(to_remove) if older_than_days else len(self.cache)} cache entries")

    def optimize_cache(self):
        """Optimize cache by removing duplicates and low-value entries"""
        # Remove exact duplicates (same prompt, different responses)
        prompt_groups = {}
        for key, response in self.cache.items():
            prompt_key = response.prompt_hash
            if prompt_key not in prompt_groups:
                prompt_groups[prompt_key] = []
            prompt_groups[prompt_key].append((key, response))

        to_remove = []
        for prompt_key, responses in prompt_groups.items():
            if len(responses) > 1:
                # Keep the highest quality response, or most recently accessed
                sorted_responses = sorted(
                    responses,
                    key=lambda x: (
                        x[1].quality_score or 0,
                        x[1].last_accessed,
                        x[1].access_count
                    ),
                    reverse=True
                )
                # Remove all but the best one
                for key, _ in sorted_responses[1:]:
                    to_remove.append(key)

        for key in to_remove:
            del self.cache[key]
            if key in self.index:
                del self.index[key]

        self._cleanup_cache()
        self._save_cache()

        print(f"Cache optimization: removed {len(to_remove)} duplicate/low-quality entries")

def main():
    parser = argparse.ArgumentParser(description='AI Response Caching System')
    parser.add_argument('command', choices=['stats', 'store', 'get', 'clear', 'optimize'],
                       help='Command to execute')
    parser.add_argument('--prompt', help='Prompt for store/get operations')
    parser.add_argument('--response', help='Response for store operation')
    parser.add_argument('--model', help='AI model used')
    parser.add_argument('--tokens', type=int, help='Tokens used')
    parser.add_argument('--cost', type=float, help='Cost incurred')
    parser.add_argument('--quality', type=float, help='Quality score (0.0-1.0)')
    parser.add_argument('--context', help='Context for the prompt')
    parser.add_argument('--older-than', type=int, help='Clear entries older than N days')

    args = parser.parse_args()

    # Initialize cache
    project_root = Path(__file__).parent.parent
    cache = AICache(project_root)

    if args.command == 'stats':
        stats = cache.get_cache_stats()
        print("🤖 AI Response Cache Statistics")
        print("=" * 40)
        print(f"Total Entries: {stats['total_entries']}")
        print(f"Estimated Cost Saved: ${stats['total_cost_saved']}")
        print(f"Estimated Tokens Saved: {stats['total_tokens_saved']:,}")
        print(f"Cache Hit Rate: {stats['cache_hit_rate']:.1%}")
        print(f"Average Quality Score: {stats['avg_quality_score']:.3f}")
        print(f"Models Used: {', '.join(stats['models_used'])}")
        print(f"Total Accesses: {stats['total_accesses']}")
        if stats['oldest_entry']:
            print(f"Oldest Entry: {stats['oldest_entry']}")
        if stats['newest_entry']:
            print(f"Newest Entry: {stats['newest_entry']}")

    elif args.command == 'store':
        if not all([args.prompt, args.response, args.model, args.tokens is not None, args.cost is not None]):
            print("Error: --prompt, --response, --model, --tokens, and --cost required for store")
            sys.exit(1)

        key = cache.store_response(
            prompt=args.prompt,
            response=args.response,
            model=args.model,
            tokens_used=args.tokens,
            cost=args.cost,
            quality_score=args.quality,
            context=args.context
        )
        print(f"✅ Stored response in cache with key: {key}")

    elif args.command == 'get':
        if not args.prompt:
            print("Error: --prompt required for get")
            sys.exit(1)

        cached = cache.get_cached_response(
            prompt=args.prompt,
            context=args.context,
            min_quality=args.quality
        )

        if cached:
            print("✅ Cache hit!")
            print(f"Model: {cached.model}")
            print(f"Tokens: {cached.tokens_used}")
            print(f"Cost: ${cached.cost:.4f}")
            print(f"Quality: {cached.quality_score or 'N/A'}")
            print(f"Response: {cached.response[:200]}{'...' if len(cached.response) > 200 else ''}")
        else:
            print("❌ No cached response found")

    elif args.command == 'clear':
        cache.clear_cache(older_than_days=args.older_than)
        print("✅ Cache cleared")

    elif args.command == 'optimize':
        cache.optimize_cache()
        print("✅ Cache optimized")

if __name__ == '__main__':
    main()