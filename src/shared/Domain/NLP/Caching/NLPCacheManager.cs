// B2X.NLP.Caching
// Advanced Caching for NLP Processing
// DocID: SPR-027-NLP-CACHING

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace B2X.NLP.Caching
{
    /// <summary>
    /// Distributed Redis-based NLP Caching Layer
    /// Optimizes cache hit rates for common NLP queries
    /// </summary>
    public class NLPCacheManager
    {
        private readonly IDatabase _redis;
        private readonly ICacheKeyGenerator _keyGenerator;
        private readonly ICacheMetrics _metrics;

        public NLPCacheManager(
            IConnectionMultiplexer redis,
            ICacheKeyGenerator keyGenerator,
            ICacheMetrics metrics)
        {
            _redis = redis.GetDatabase();
            _keyGenerator = keyGenerator;
            _metrics = metrics;
        }

        /// <summary>
        /// Get cached NLP result or compute and cache
        /// </summary>
        public async Task<NLPCacheResult<T>> GetOrComputeAsync<T>(
            string operation,
            object input,
            Func<Task<T>> computeFunc,
            TimeSpan? expiry = null)
        {
            var key = _keyGenerator.GenerateKey(operation, input);

            // Try cache first
            var cached = await _redis.StringGetAsync(key);
            if (cached.HasValue)
            {
                _metrics.RecordHit();
                return new NLPCacheResult<T>
                {
                    Value = Deserialize<T>(cached),
                    IsFromCache = true
                };
            }

            _metrics.RecordMiss();

            // Compute result
            var result = await computeFunc();

            // Cache result
            await _redis.StringSetAsync(key, Serialize(result), expiry ?? TimeSpan.FromHours(1));

            return new NLPCacheResult<T>
            {
                Value = result,
                IsFromCache = false
            };
        }

        /// <summary>
        /// Invalidate cache patterns
        /// </summary>
        public async Task InvalidatePatternAsync(string pattern)
        {
            var keys = new List<RedisKey>();
            var endpoints = _redis.Multiplexer.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                var server = _redis.Multiplexer.GetServer(endpoint);
                keys.AddRange(server.Keys(pattern: pattern));
            }

            if (keys.Any())
            {
                await _redis.KeyDeleteAsync(keys.ToArray());
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public CacheStats GetStats()
        {
            return _metrics.GetStats();
        }

        private string Serialize<T>(T obj) => System.Text.Json.JsonSerializer.Serialize(obj);
        private T Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    public interface ICacheKeyGenerator
    {
        string GenerateKey(string operation, object input);
    }

    public interface ICacheMetrics
    {
        void RecordHit();
        void RecordMiss();
        CacheStats GetStats();
    }

    public class NLPCacheResult<T>
    {
        public T Value { get; set; }
        public bool IsFromCache { get; set; }
    }

    public class CacheStats
    {
        public long TotalRequests { get; set; }
        public long CacheHits { get; set; }
        public long CacheMisses { get; set; }
        public double HitRate => TotalRequests > 0 ? (double)CacheHits / TotalRequests : 0;
        public long KeysCount { get; set; }
    }

    /// <summary>
    /// Cache key generator with model versioning
    /// </summary>
    public class ModelVersionedKeyGenerator : ICacheKeyGenerator
    {
        private readonly string _modelVersion;

        public ModelVersionedKeyGenerator(string modelVersion)
        {
            _modelVersion = modelVersion;
        }

        public string GenerateKey(string operation, object input)
        {
            var inputHash = GetHash(input);
            return $"nlp:{_modelVersion}:{operation}:{inputHash}";
        }

        private string GetHash(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hash).Substring(0, 16);
        }
    }
}