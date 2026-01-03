using B2Connect.Admin.Tests.Benchmarks;
using BenchmarkDotNet.Running;

// ─────────────────────────────────────────────────────────────────────────────
// ADR-025 Performance Benchmarks Runner - Phase 5
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Console application to run ADR-025 performance benchmarks
/// Tests Dapper vs EF Core performance for bulk operations
/// </summary>
internal partial class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("🚀 Running ADR-025 Phase 5 Performance Benchmarks");
        Console.WriteLine("📊 Testing Dapper vs EF Core Bulk Operations");
        Console.WriteLine();

        var summary = BenchmarkRunner.Run<BulkImportBenchmarks>();

        Console.WriteLine();
        Console.WriteLine("✅ Benchmarks completed successfully!");
        Console.WriteLine("📈 Check the BenchmarkDotNet output above for performance results");
    }
}
