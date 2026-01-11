# Performance MCP Server

A Model Context Protocol (MCP) server for code performance analysis, profiling, memory optimization, and benchmark generation. This server helps identify performance bottlenecks and optimization opportunities across different programming languages.

## Features

### Code Performance Analysis
- **Algorithm Complexity**: Identifies inefficient algorithms and data structures
- **Loop Optimization**: Analyzes loops for performance bottlenecks
- **Memory Usage**: Detects memory leaks and inefficient allocations
- **Async Performance**: Reviews asynchronous code patterns
- **Language-Specific**: Supports TypeScript, JavaScript, C#, and Python

### Memory Profiling
- **Memory Leaks**: Identifies potential memory leaks in event listeners, timers, and subscriptions
- **Resource Management**: Checks for proper cleanup of resources
- **Allocation Patterns**: Analyzes object creation and garbage collection patterns
- **Cross-Language**: Memory analysis for different programming paradigms

### Loop Optimization
- **Loop Efficiency**: Identifies slow loop patterns and suggests optimizations
- **Iteration Methods**: Recommends better iteration approaches
- **Caching Strategies**: Suggests caching array lengths and other optimizations
- **Language Idioms**: Provides language-specific optimization recommendations

### Bundle Size Analysis
- **Build Configuration**: Analyzes build tools and configuration
- **Import Optimization**: Identifies excessive imports and suggests lazy loading
- **Code Splitting**: Recommends bundle splitting strategies
- **Asset Analysis**: Reviews large assets and inline data

### Performance Benchmarking
- **Code Snippets**: Benchmarks provided code snippets
- **Comparative Analysis**: Compares different implementation approaches
- **Performance Metrics**: Provides execution time and memory usage data
- **Optimization Validation**: Validates performance improvements

## Installation

```bash
cd tools/PerformanceMCP
npm install
npm run build
```

## Configuration

Add to your `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "performance-mcp": {
      "command": "node",
      "args": [
        "tools/PerformanceMCP/dist/index.js"
      ],
      "env": {
        "NODE_ENV": "production"
      },
      "disabled": false
    }
  }
}
```

## Usage

### Analyze Code Performance

```typescript
// Analyze TypeScript/JavaScript performance
performance-mcp/analyze_code_performance workspacePath="frontend" language="typescript" focus="all"

// Focus on specific aspects
performance-mcp/analyze_code_performance workspacePath="backend" language="csharp" focus="loops"

// Analyze Python code
performance-mcp/analyze_code_performance workspacePath="scripts" language="python" focus="memory"
```

### Profile Memory Usage

```typescript
// Check for memory leaks in frontend code
performance-mcp/profile_memory_usage workspacePath="frontend" language="typescript"

// Analyze C# memory management
performance-mcp/profile_memory_usage workspacePath="backend" language="csharp"
```

### Optimize Loops

```typescript
// Analyze loop performance
performance-mcp/optimize_loops workspacePath="backend" language="csharp"

// Check JavaScript loop patterns
performance-mcp/optimize_loops workspacePath="frontend" language="typescript"
```

### Check Bundle Size

```typescript
// Analyze bundle configuration
performance-mcp/check_bundle_size workspacePath="frontend" targetSize=500

// Check specific build config
performance-mcp/check_bundle_size workspacePath="frontend" buildConfig="vite.config.ts"
```

### Performance Benchmarks

```typescript
// Benchmark a code snippet
performance-mcp/performance_benchmarks codeSnippet="for(let i=0; i<arr.length; i++) { sum += arr[i]; }" language="typescript" iterations=1000

// Compare implementations
performance-mcp/performance_benchmarks codeSnippet="arr.reduce((sum, val) => sum + val, 0)" language="typescript"
```

## Performance Issues Detected

### JavaScript/TypeScript
- **Loop Inefficiencies**: `for (let i = 0; i < arr.length; i++)` without caching length
- **Memory Leaks**: Event listeners without cleanup, uncleared intervals
- **Inefficient Operations**: `JSON.parse/stringify` for deep cloning
- **Console Statements**: Leftover debug statements in production

### C#
- **LINQ Performance**: Calling `ToList()` before filtering operations
- **String Concatenation**: Using `+=` in loops without `StringBuilder`
- **Collection Initialization**: Inefficient list creation patterns

### Python
- **Range Iteration**: Using `range(len())` instead of `enumerate()`
- **List Operations**: Using `append()` in loops instead of comprehensions
- **Global Access**: Accessing global variables in performance-critical code

## Optimization Recommendations

### General Best Practices
- **Cache Array Lengths**: Store `arr.length` in a variable before loops
- **Use Appropriate Data Structures**: Choose the right collection type for your use case
- **Minimize Object Creation**: Reuse objects when possible
- **Lazy Loading**: Load resources only when needed

### Language-Specific Optimizations
- **JavaScript**: Use `for-of` loops, avoid `for-in` for arrays
- **C#**: Use `ArrayPool<T>` for large arrays, prefer `Span<T>` for slicing
- **Python**: Use list comprehensions, avoid global variable access in loops

## Integration with B2X

This MCP server integrates with the B2X development workflow to provide:

- **Pre-commit Performance Checks**: Automatic performance analysis before commits
- **Code Review Performance**: Performance impact assessment during reviews
- **Build Performance Monitoring**: Bundle size and build time optimization
- **Production Monitoring**: Runtime performance profiling integration

## Performance Metrics

The server provides various performance metrics:

- **Cyclomatic Complexity**: Code complexity measurements
- **Loop Count**: Number of loops and iteration patterns
- **Async Operations**: Count of asynchronous functions
- **Memory Patterns**: Potential memory leak indicators
- **Bundle Size Estimates**: Rough bundle size calculations

## Contributing

1. Follow the existing code patterns in other MCP servers
2. Add comprehensive tests for new performance checks
3. Update this README with new analysis capabilities
4. Ensure TypeScript strict mode compliance

## License

MIT