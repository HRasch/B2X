using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace B2Connect.ErpConnector.Services
{
    /// <summary>
    /// Performance monitoring service for ERP connector operations.
    /// Tracks execution times, throughput, and error rates for benchmarking and monitoring.
    /// </summary>
    public class PerformanceMonitor : IDisposable
    {
        private readonly ConcurrentDictionary<string, OperationMetrics> _metrics = new ConcurrentDictionary<string, OperationMetrics>();
        private readonly Stopwatch _sessionStopwatch = Stopwatch.StartNew();
        private readonly Timer _reportingTimer;
        private readonly bool _enabled;
        private readonly object _lock = new object();

        public PerformanceMonitor(bool enabled = true, TimeSpan? reportingInterval = null)
        {
            _enabled = enabled;
            if (_enabled && reportingInterval.HasValue)
            {
                _reportingTimer = new Timer(ReportMetrics, null, reportingInterval.Value, reportingInterval.Value);
            }
        }

        /// <summary>
        /// Records the execution time of an operation
        /// </summary>
        public void RecordOperation(string operationName, TimeSpan duration, bool success = true)
        {
            if (!_enabled) return;

            var metrics = _metrics.GetOrAdd(operationName, _ => new OperationMetrics(operationName));
            metrics.Record(duration, success);
        }

        /// <summary>
        /// Times an operation and records its performance
        /// </summary>
        public T TimeOperation<T>(string operationName, Func<T> operation)
        {
            if (!_enabled) return operation();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = operation();
                stopwatch.Stop();
                RecordOperation(operationName, stopwatch.Elapsed, true);
                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                RecordOperation(operationName, stopwatch.Elapsed, false);
                throw;
            }
        }

        /// <summary>
        /// Times an async operation and records its performance
        /// </summary>
        public async System.Threading.Tasks.Task<T> TimeOperationAsync<T>(string operationName, Func<System.Threading.Tasks.Task<T>> operation)
        {
            if (!_enabled) return await operation();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await operation();
                stopwatch.Stop();
                RecordOperation(operationName, stopwatch.Elapsed, true);
                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                RecordOperation(operationName, stopwatch.Elapsed, false);
                throw;
            }
        }

        /// <summary>
        /// Gets metrics for a specific operation
        /// </summary>
        public OperationMetrics GetMetrics(string operationName)
        {
            return _metrics.TryGetValue(operationName, out var metrics) ? metrics : null;
        }

        /// <summary>
        /// Gets all operation metrics
        /// </summary>
        public IEnumerable<OperationMetrics> GetAllMetrics()
        {
            return _metrics.Values.ToList();
        }

        /// <summary>
        /// Reports current metrics to console
        /// </summary>
        public void ReportMetrics(object state = null)
        {
            if (!_enabled) return;

            lock (_lock)
            {
                Console.WriteLine("\n=== PERFORMANCE METRICS ===");
                Console.WriteLine($"Session Time: {_sessionStopwatch.Elapsed.TotalSeconds:F2}s");

                foreach (var metrics in _metrics.Values.OrderBy(m => m.OperationName))
                {
                    Console.WriteLine(metrics.ToString());
                }
                Console.WriteLine("===========================\n");
            }
        }

        /// <summary>
        /// Exports metrics in a structured format for analysis
        /// </summary>
        public string ExportMetrics()
        {
            if (!_enabled) return "{}";

            var export = new
            {
                SessionTimeSeconds = _sessionStopwatch.Elapsed.TotalSeconds,
                Operations = _metrics.Values.Select(m => new
                {
                    m.OperationName,
                    m.TotalCalls,
                    m.SuccessfulCalls,
                    m.FailedCalls,
                    AverageDurationMs = m.AverageDuration.TotalMilliseconds,
                    MinDurationMs = m.MinDuration.TotalMilliseconds,
                    MaxDurationMs = m.MaxDuration.TotalMilliseconds,
                    TotalDurationMs = m.TotalDuration.TotalMilliseconds,
                    CallsPerSecond = m.CallsPerSecond,
                    ErrorRate = m.ErrorRate
                }).ToList()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(export, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Resets all metrics
        /// </summary>
        public void Reset()
        {
            _metrics.Clear();
            _sessionStopwatch.Restart();
        }

        public void Dispose()
        {
            _reportingTimer?.Dispose();
            ReportMetrics(); // Final report
        }
    }

    /// <summary>
    /// Metrics for a specific operation
    /// </summary>
    public class OperationMetrics
    {
        private readonly object _lock = new object();
        private long _totalCalls;
        private long _successfulCalls;
        private TimeSpan _totalDuration;
        private TimeSpan _minDuration = TimeSpan.MaxValue;
        private TimeSpan _maxDuration = TimeSpan.MinValue;
        private readonly Stopwatch _operationStopwatch = Stopwatch.StartNew();

        public string OperationName { get; }
        public long TotalCalls => _totalCalls;
        public long SuccessfulCalls => _successfulCalls;
        public long FailedCalls => _totalCalls - _successfulCalls;
        public TimeSpan TotalDuration => _totalDuration;
        public TimeSpan AverageDuration => _totalCalls > 0 ? TimeSpan.FromTicks(_totalDuration.Ticks / _totalCalls) : TimeSpan.Zero;
        public TimeSpan MinDuration => _minDuration == TimeSpan.MaxValue ? TimeSpan.Zero : _minDuration;
        public TimeSpan MaxDuration => _maxDuration == TimeSpan.MinValue ? TimeSpan.Zero : _maxDuration;
        public double CallsPerSecond => _operationStopwatch.Elapsed.TotalSeconds > 0 ? _totalCalls / _operationStopwatch.Elapsed.TotalSeconds : 0;
        public double ErrorRate => _totalCalls > 0 ? (double)FailedCalls / _totalCalls : 0;

        public OperationMetrics(string operationName)
        {
            OperationName = operationName;
        }

        public void Record(TimeSpan duration, bool success)
        {
            lock (_lock)
            {
                _totalCalls++;
                _totalDuration += duration;

                if (success)
                    _successfulCalls++;

                if (duration < _minDuration)
                    _minDuration = duration;

                if (duration > _maxDuration)
                    _maxDuration = duration;
            }
        }

        public override string ToString()
        {
            return $"{OperationName}: {_totalCalls} calls, avg={AverageDuration.TotalMilliseconds:F2}ms, min={MinDuration.TotalMilliseconds:F2}ms, max={MaxDuration.TotalMilliseconds:F2}ms, {CallsPerSecond:F2} calls/sec, error={ErrorRate:P2}";
        }
    }
}