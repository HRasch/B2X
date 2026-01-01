# Database Optimization Best Practices

This document compiles comprehensive best practices for database optimization across SQL Server, PostgreSQL, MongoDB, and Entity Framework (EF) with extensions. It focuses on query optimization, performance tuning, indexing strategies, and EF-specific optimizations. The content is derived from authoritative sources including Microsoft Docs, PostgreSQL Docs, MongoDB Docs, and EF documentation.

## SQL Server Best Practices

### Query Optimization
- **Use Execution Plans**: Analyze query execution plans using `SHOWPLAN_XML` or graphical execution plans in SQL Server Management Studio. Identify costly operations like table scans vs. index seeks.
- **Avoid Cartesian Products**: Ensure joins are properly defined to prevent N x M result sets.
- **Use Appropriate Join Types**: Prefer merge joins for sorted data, hash joins for large unsorted datasets, and nested loops for small result sets.
- **Minimize Subqueries**: Replace correlated subqueries with joins or derived tables where possible.
- **Use Query Hints Sparingly**: Only use hints like `MAXDOP`, `OPTIMIZE FOR`, or `RECOMPILE` when necessary, as they can override optimizer decisions.

**Example**: For a query with multiple joins, use `EXPLAIN` to check the plan:
```sql
EXPLAIN SELECT * FROM Orders o JOIN Customers c ON o.CustomerID = c.CustomerID WHERE o.OrderDate > '2023-01-01';
```

**Links**:
- [Query Processing Architecture Guide](https://learn.microsoft.com/en-us/sql/relational-databases/query-processing-architecture-guide)
- [Performance Center](https://learn.microsoft.com/en-us/sql/relational-databases/performance/performance-center-for-sql-server-database-engine-and-azure-sql-database)

### Indexing Strategies
- **Clustered Indexes**: Use on primary key columns or frequently queried columns for physical data ordering.
- **Non-Clustered Indexes**: Create on columns used in WHERE, JOIN, ORDER BY, and GROUP BY clauses.
- **Covering Indexes**: Include all columns needed in a query to avoid key lookups.
- **Filtered Indexes**: For selective data subsets, e.g., `CREATE INDEX idx_active_customers ON Customers(LastName) WHERE IsActive = 1;`
- **Columnstore Indexes**: For analytical workloads on large datasets to achieve compression and query performance gains.
- **Index Maintenance**: Regularly rebuild or reorganize indexes based on fragmentation levels.

**Example**: Create a covering index for a common query:
```sql
CREATE NONCLUSTERED INDEX idx_orders_covering ON Orders(OrderDate, CustomerID) INCLUDE(TotalAmount);
```

**Links**:
- [Indexes Overview](https://learn.microsoft.com/en-us/sql/relational-databases/indexes/indexes)
- [SQL Server Index Design Guide](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-index-design-guide)

### Performance Tuning
- **Statistics**: Keep statistics updated for accurate cardinality estimation. Use `UPDATE STATISTICS` or enable auto-update.
- **Partitioning**: Use table partitioning for large tables to improve query performance and maintenance.
- **Memory Configuration**: Adjust `max server memory` and `min memory per query` settings.
- **Parallelism**: Configure `max degree of parallelism (MAXDOP)` appropriately (often 4-8 for modern servers).
- **TempDB Optimization**: Place TempDB on fast storage, use multiple files, and monitor for contention.
- **Query Store**: Enable Query Store to track query performance over time and identify regressions.

**Example**: Enable Query Store:
```sql
ALTER DATABASE YourDatabase SET QUERY_STORE = ON;
```

**Links**:
- [Performance Center](https://learn.microsoft.com/en-us/sql/relational-databases/performance/performance-center-for-sql-server-database-engine-and-azure-sql-database)
- [Intelligent Query Processing](https://learn.microsoft.com/en-us/sql/relational-databases/performance/intelligent-query-processing)

## PostgreSQL Best Practices

### Query Optimization
- **Use EXPLAIN ANALYZE**: Always analyze query plans with `EXPLAIN ANALYZE` to see actual execution times and row counts.
- **Avoid Sequential Scans**: Ensure indexes are used; use `VACUUM ANALYZE` to update statistics.
- **Optimize Joins**: Prefer hash joins for large tables, nested loops for small result sets.
- **Use CTEs Wisely**: Common Table Expressions can improve readability but may impact performance; consider materialization.
- **Limit Result Sets**: Use `LIMIT` early in queries to reduce processing.
- **Avoid Functions in WHERE Clauses**: Functions on columns prevent index usage; use computed columns or rewrite queries.

**Example**: Analyze a query plan:
```sql
EXPLAIN ANALYZE SELECT * FROM orders WHERE order_date > '2023-01-01' AND customer_id = 123;
```

**Links**:
- [Using EXPLAIN](https://www.postgresql.org/docs/current/using-explain.html)
- [Performance Tips](https://www.postgresql.org/docs/current/performance-tips.html)

### Indexing Strategies
- **B-Tree Indexes**: Default for most cases; supports equality, range, and ordering.
- **Hash Indexes**: For simple equality lookups (fast but not WAL-logged until PG 10+).
- **GiST/GIN/SP-GiST Indexes**: For complex data types like arrays, full-text, or geometric data.
- **Partial Indexes**: Index only a subset of rows, e.g., `CREATE INDEX idx_active_users ON users(email) WHERE active = true;`
- **Expression Indexes**: Index on computed expressions, e.g., `CREATE INDEX idx_lower_email ON users(lower(email));`
- **Multicolumn Indexes**: For queries filtering on multiple columns; order matters for selectivity.

**Example**: Create a partial index:
```sql
CREATE INDEX idx_recent_orders ON orders(order_date) WHERE order_date > '2023-01-01';
```

**Links**:
- [Indexes](https://www.postgresql.org/docs/current/indexes.html)
- [Multicolumn Indexes](https://www.postgresql.org/docs/current/indexes-multicolumn.html)

### Performance Tuning
- **Configuration**: Tune `shared_buffers`, `work_mem`, `maintenance_work_mem`, and `effective_cache_size`.
- **VACUUM and ANALYZE**: Regularly run to reclaim space and update statistics.
- **Connection Pooling**: Use PgBouncer or similar to manage connections.
- **Partitioning**: Use table partitioning for large tables (available since PG 10).
- **Query Planning**: Adjust `random_page_cost` and `seq_page_cost` for storage types.
- **Monitoring**: Use `pg_stat_statements` extension for query performance tracking.

**Example**: Increase work memory for complex queries:
```sql
SET work_mem = '64MB';
```

**Links**:
- [Performance Tips](https://www.postgresql.org/docs/current/performance-tips.html)
- [Planner Statistics](https://www.postgresql.org/docs/current/planner-stats.html)

## MongoDB Best Practices

### Query Optimization
- **Use Covered Queries**: Ensure queries can be satisfied entirely from indexes.
- **Avoid Unnecessary Fields**: Use projection to return only needed fields.
- **Optimize Aggregation Pipelines**: Use `$match` early to reduce documents processed.
- **Use Explain Plans**: Run `db.collection.explain("executionStats")` to analyze query performance.
- **Avoid Regex on Large Collections**: Use text indexes for text searches.
- **Batch Operations**: Use bulk operations for multiple inserts/updates.

**Example**: Covered query:
```javascript
db.users.find({age: {$gt: 18}}, {name: 1, _id: 0}).explain("executionStats");
```

**Links**:
- [Query Optimization](https://docs.mongodb.com/manual/core/query-optimization/)
- [Query Plans](https://docs.mongodb.com/manual/core/query-plans/)

### Indexing Strategies
- **Single Field Indexes**: For equality matches, range queries, and sorts.
- **Compound Indexes**: For queries on multiple fields; order fields by selectivity.
- **Multikey Indexes**: For arrays; limit array size to avoid performance issues.
- **Text Indexes**: For full-text search on string fields.
- **Geospatial Indexes**: For location-based queries (2d, 2dsphere).
- **Hashed Indexes**: For sharding; good for equality but not range queries.
- **Partial Indexes**: Index only documents matching a filter expression.
- **Index Maintenance**: Monitor index usage with `$indexStats`; drop unused indexes.

**Example**: Create a compound index:
```javascript
db.orders.createIndex({customer_id: 1, order_date: -1});
```

**Links**:
- [Indexes](https://docs.mongodb.com/manual/indexes/)
- [Index Strategies](https://docs.mongodb.com/manual/core/indexes/)

### Performance Tuning
- **Read Preferences**: Use secondary reads for non-critical queries.
- **Write Concerns**: Balance durability with performance (e.g., w:1 for fast writes).
- **Sharding**: Distribute data across shards for horizontal scaling.
- **Connection Pooling**: Configure connection pools in drivers.
- **Profiling**: Enable database profiler to log slow queries.
- **Memory Management**: Ensure working set fits in RAM; monitor with `db.serverStatus()`.

**Example**: Enable profiling for slow queries:
```javascript
db.setProfilingLevel(1, {slowms: 100});
```

**Links**:
- [Query Optimization](https://docs.mongodb.com/manual/core/query-optimization/)
- [Performance Best Practices](https://docs.mongodb.com/manual/core/query-optimization/#performance-best-practices)

## Entity Framework Best Practices

### Query Optimization
- **Use AsNoTracking**: For read-only queries to disable change tracking.
- **Select Only Needed Data**: Use `Select` to project only required properties.
- **Avoid N+1 Queries**: Use `Include` or `ThenInclude` for eager loading, or explicit joins.
- **Use Compiled Queries**: For frequently executed queries to cache compilation.
- **Batch Operations**: Use `AddRange`, `RemoveRange` for bulk operations.
- **Async Operations**: Use async methods to avoid blocking threads.

**Example**: Eager loading to avoid N+1:
```csharp
var orders = context.Orders.Include(o => o.Customer).Where(o => o.Total > 100).ToList();
```

**Links**:
- [Querying Data](https://learn.microsoft.com/en-us/ef/core/querying/)
- [Performance Introduction](https://learn.microsoft.com/en-us/ef/core/performance/)

### EF-Specific Optimizations
- **Disable Auto-Detect Changes**: Use `AutoDetectChangesEnabled = false` for bulk updates.
- **Use Raw SQL**: For complex queries where LINQ is inefficient.
- **Optimize Migrations**: Use `EnsureCreated` for development; script migrations for production.
- **Connection Resiliency**: Configure retry logic for transient failures.
- **Caching**: Implement second-level caching with extensions like EFCoreSecondLevelCacheInterceptor.
- **Lazy Loading**: Enable only when needed; prefer explicit loading.

**Example**: Disable change tracking for bulk insert:
```csharp
context.ChangeTracker.AutoDetectChangesEnabled = false;
context.AddRange(entities);
context.SaveChanges();
```

**Links**:
- [Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
- [Extensions](https://learn.microsoft.com/en-us/ef/core/extensions/)

### Extensions for Optimization
- **EFCore.BulkExtensions**: For high-performance bulk operations (insert, update, delete).
- **EFCoreSecondLevelCacheInterceptor**: Caches query results to reduce database hits.
- **EntityFrameworkPlus**: Provides batch operations, auditing, and caching.
- **linq2db.EntityFrameworkCore**: Alternative LINQ translator for advanced SQL features.
- **EFCore.NamingConventions**: Automatically applies naming conventions to improve portability.

**Example**: Using bulk extensions:
```csharp
context.BulkInsert(entities);
```

**Links**:
- [EF Core Extensions](https://learn.microsoft.com/en-us/ef/core/extensions/)
- [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions)

This summary provides a foundation for optimizing database performance. Always measure and profile in your specific environment, as results can vary based on data, hardware, and workload. For the new database specialist agent, integrate these practices into `.ai/knowledgebase/database-optimization.md` for quick reference.

## Common Database Design Antipatterns and Pitfalls

This section outlines common design mistakes, performance issues, scalability problems, and maintenance challenges in SQL databases (SQL Server, PostgreSQL) and NoSQL databases (MongoDB). Each antipattern includes examples, consequences, and solutions, drawing from authoritative sources.

### SQL Databases (SQL Server, PostgreSQL)

#### Antipattern: Over-Normalization (Excessive Normalization)
**Description**: Breaking data into too many tables, leading to complex joins and poor read performance.

**Example**: Storing user addresses in separate tables for street, city, state, country, each with foreign keys, requiring 4-5 joins for a simple address query.

**Consequences**:
- Increased query complexity and execution time due to multiple joins
- Higher maintenance overhead for schema changes
- Potential for performance degradation in read-heavy workloads

**Solutions**:
- Use appropriate normalization levels (typically 3NF for OLTP)
- Consider denormalization for read-heavy tables with computed columns
- Implement views or materialized views for complex joins

**Sources**:
- [Database Normalization](https://en.wikipedia.org/wiki/Database_normalization)
- [PostgreSQL: Database Design](https://www.postgresql.org/docs/current/ddl.html)
- [SQL Server: Normalization](https://learn.microsoft.com/en-us/sql/relational-databases/tables/primary-and-foreign-key-constraints)

#### Antipattern: Under-Normalization (Denormalization Abuse)
**Description**: Storing redundant data to avoid joins, leading to update anomalies and data inconsistency.

**Example**: Storing customer name in both Orders and Invoices tables, causing inconsistencies when customer data changes.

**Consequences**:
- Data integrity issues and update anomalies
- Increased storage requirements
- Complex update logic to maintain consistency

**Solutions**:
- Use triggers or application logic to maintain denormalized data
- Implement computed columns for derived data
- Consider using database views for virtual denormalization

**Sources**:
- [Database Normalization Basics](https://www.lucidchart.com/pages/database-normalization)
- [SQL Server: Computed Columns](https://learn.microsoft.com/en-us/sql/relational-databases/tables/specify-computed-columns-in-a-table)

#### Antipattern: Missing or Inappropriate Primary Keys
**Description**: Using non-unique or composite keys that don't reflect business logic, or missing primary keys entirely.

**Example**: Using email as primary key for users, which can change, or using multi-column composite keys for simple entities.

**Consequences**:
- Difficulty in establishing relationships
- Performance issues with joins and foreign keys
- Data integrity problems

**Solutions**:
- Use surrogate keys (auto-incrementing IDs) for technical primary keys
- Implement unique constraints on business keys
- Ensure primary keys are immutable

**Sources**:
- [Primary Key Design](https://www.red-gate.com/simple-talk/databases/sql-server/database-design/sql-server-primary-key-design/)
- [PostgreSQL: Primary Keys](https://www.postgresql.org/docs/current/ddl-constraints.html#DDL-CONSTRAINTS-PRIMARY-KEYS)

#### Antipattern: Large Text Fields in Main Tables
**Description**: Storing large BLOB/CLOB data directly in frequently queried tables.

**Example**: Storing product images or large descriptions in the Products table alongside metadata.

**Consequences**:
- Increased table size and memory usage
- Slower queries due to data transfer overhead
- Index inefficiency

**Solutions**:
- Store large objects separately with references
- Use FILESTREAM in SQL Server or Large Objects in PostgreSQL
- Implement table partitioning for large tables

**Sources**:
- [SQL Server: FILESTREAM](https://learn.microsoft.com/en-us/sql/relational-databases/blob/filestream-sql-server)
- [PostgreSQL: Large Objects](https://www.postgresql.org/docs/current/largeobjects.html)

#### Antipattern: Ignoring Indexing Best Practices
**Description**: Creating too many indexes, missing critical indexes, or using inappropriate index types.

**Example**: Indexing every column in a table or missing indexes on foreign key columns.

**Consequences**:
- Slow write operations due to index maintenance
- Wasted storage space
- Query performance issues

**Solutions**:
- Analyze query patterns before indexing
- Use covering indexes for common query patterns
- Regularly monitor and maintain indexes

**Sources**:
- [SQL Server Index Design](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-index-design-guide)
- [PostgreSQL: Indexes](https://www.postgresql.org/docs/current/indexes.html)

#### Antipattern: Poor Data Type Choices
**Description**: Using inappropriate data types that waste space or limit functionality.

**Example**: Using VARCHAR(MAX) for all string fields or INT for dates.

**Consequences**:
- Increased storage requirements
- Performance overhead
- Limited query capabilities

**Solutions**:
- Choose appropriate data types based on data characteristics
- Use domain-specific types (e.g., UUID, JSONB in PostgreSQL)
- Consider storage implications

**Sources**:
- [SQL Server Data Types](https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql)
- [PostgreSQL Data Types](https://www.postgresql.org/docs/current/datatype.html)

#### Antipattern: Lack of Constraints
**Description**: Missing foreign key constraints, check constraints, or unique constraints.

**Example**: Allowing invalid data states, like negative prices or orphaned records.

**Consequences**:
- Data integrity issues
- Application-level validation complexity
- Difficult debugging

**Solutions**:
- Implement appropriate constraints at the database level
- Use triggers for complex business rules
- Regularly validate data integrity

**Sources**:
- [SQL Server Constraints](https://learn.microsoft.com/en-us/sql/relational-databases/tables/unique-constraints-and-check-constraints)
- [PostgreSQL Constraints](https://www.postgresql.org/docs/current/ddl-constraints.html)

### NoSQL Databases (MongoDB)

#### Antipattern: Embedding vs. Referencing Confusion
**Description**: Incorrectly embedding documents when referencing would be better, or vice versa.

**Example**: Embedding all order items in the order document for a large e-commerce site, leading to document size limits.

**Consequences**:
- Document size exceeding 16MB limit in MongoDB
- Read performance issues for large documents
- Update complexity for embedded data

**Solutions**:
- Use references for one-to-many relationships with high cardinality
- Embed for one-to-one or one-to-few relationships
- Consider hybrid approaches with manual referencing

**Sources**:
- [MongoDB Data Modeling](https://docs.mongodb.com/manual/core/data-modeling-introduction/)
- [Schema Design Patterns](https://docs.mongodb.com/manual/applications/data-models/)

#### Antipattern: Unbounded Arrays
**Description**: Allowing arrays to grow without limits in documents.

**Example**: Storing all user actions in an array within the user document, leading to performance degradation.

**Consequences**:
- Document growth beyond working set
- Query performance degradation
- Memory pressure

**Solutions**:
- Implement array size limits
- Use separate collections for unbounded relationships
- Implement data archiving strategies

**Sources**:
- [MongoDB Limits and Thresholds](https://docs.mongodb.com/manual/reference/limits/)
- [Data Modeling Considerations](https://docs.mongodb.com/manual/core/data-model-design/)

#### Antipattern: Missing Indexes on Query Fields
**Description**: Failing to index fields commonly used in queries, filters, or sorts.

**Example**: Querying on non-indexed fields in large collections, causing collection scans.

**Consequences**:
- Slow query performance
- High CPU and I/O usage
- Poor user experience

**Solutions**:
- Analyze query patterns and create appropriate indexes
- Use compound indexes for multi-field queries
- Monitor slow queries with the profiler

**Sources**:
- [MongoDB Indexing](https://docs.mongodb.com/manual/indexes/)
- [Query Optimization](https://docs.mongodb.com/manual/core/query-optimization/)

#### Antipattern: Ignoring Schema Design for Access Patterns
**Description**: Designing schema based on relational thinking rather than application access patterns.

**Example**: Creating normalized collections that require multiple queries for common operations.

**Consequences**:
- Increased number of database round trips
- Complex aggregation pipelines
- Poor performance for common operations

**Solutions**:
- Design schema based on read/write patterns
- Use embedding for frequently accessed related data
- Optimize for the most common queries

**Sources**:
- [Data Modeling Introduction](https://docs.mongodb.com/manual/core/data-modeling-introduction/)
- [Schema Design Anti-Patterns](https://www.mongodb.com/blog/post/schema-design-anti-patterns)

#### Antipattern: Large Documents with Frequent Updates
**Description**: Creating documents that are updated frequently and contain large amounts of data.

**Example**: Storing user profiles with large embedded objects that change often.

**Consequences**:
- Write amplification due to document-level locking
- Increased journal and oplog size
- Replication lag

**Solutions**:
- Separate frequently changing data into different documents
- Use field-level updates with $set operators
- Implement document versioning strategies

**Sources**:
- [MongoDB Storage](https://docs.mongodb.com/manual/core/storage/)
- [Update Operations](https://docs.mongodb.com/manual/crud/#update-operations)

#### Antipattern: Improper Sharding Key Selection
**Description**: Choosing sharding keys that don't distribute data evenly or match query patterns.

**Example**: Sharding on a low-cardinality field like status, causing hotspots.

**Consequences**:
- Uneven data distribution
- Performance bottlenecks on specific shards
- Inefficient queries requiring scatter-gather

**Solutions**:
- Choose high-cardinality, frequently queried fields as shard keys
- Use compound shard keys for better distribution
- Test sharding strategy with representative data

**Sources**:
- [Sharding](https://docs.mongodb.com/manual/sharding/)
- [Shard Key Selection](https://docs.mongodb.com/manual/core/sharding-shard-key/)

#### Antipattern: Neglecting Data Consistency Requirements
**Description**: Using MongoDB's flexible schema to ignore consistency needs of the application.

**Example**: Allowing inconsistent data formats across documents in the same collection.

**Consequences**:
- Application complexity in handling varied data formats
- Difficult querying and aggregation
- Data quality issues

**Solutions**:
- Define and enforce schema standards
- Use schema validation rules
- Implement application-level data validation

**Sources**:
- [Schema Validation](https://docs.mongodb.com/manual/core/schema-validation/)
- [Data Modeling](https://docs.mongodb.com/manual/core/data-modeling-introduction/)

### General Antipatterns Across Database Types

#### Antipattern: Lack of Monitoring and Profiling
**Description**: Failing to monitor database performance and query patterns.

**Example**: Not tracking slow queries or resource usage until problems occur.

**Consequences**:
- Undetected performance degradation
- Difficulty in troubleshooting issues
- Reactive rather than proactive optimization

**Solutions**:
- Implement comprehensive monitoring (SQL Server Extended Events, PostgreSQL pg_stat_statements, MongoDB profiler)
- Set up alerts for performance thresholds
- Regularly review and optimize based on metrics

**Sources**:
- [SQL Server Monitoring](https://learn.microsoft.com/en-us/sql/relational-databases/performance/monitor-and-tune-for-performance)
- [PostgreSQL Monitoring](https://www.postgresql.org/docs/current/monitoring.html)
- [MongoDB Monitoring](https://docs.mongodb.com/manual/administration/monitoring/)

#### Antipattern: Ignoring Backup and Recovery Planning
**Description**: Inadequate backup strategies or lack of disaster recovery planning.

**Example**: No regular backups or untested recovery procedures.

**Consequences**:
- Data loss in case of failures
- Extended downtime during recovery
- Compliance and regulatory issues

**Solutions**:
- Implement regular backup schedules
- Test backup restoration procedures
- Design for high availability and disaster recovery

**Sources**:
- [SQL Server Backup](https://learn.microsoft.com/en-us/sql/relational-databases/backup-restore/back-up-and-restore-of-sql-server-databases)
- [PostgreSQL Backup](https://www.postgresql.org/docs/current/backup.html)
- [MongoDB Backup](https://docs.mongodb.com/manual/core/backups/)

#### Antipattern: Poor Query Design
**Description**: Writing inefficient queries that don't leverage database capabilities.

**Example**: Using application-side loops instead of set-based operations in SQL, or multiple round trips in NoSQL.

**Consequences**:
- Poor performance and scalability
- Increased network overhead
- Resource waste

**Solutions**:
- Use set-based operations in SQL
- Leverage aggregation frameworks in NoSQL
- Optimize queries based on execution plans

**Sources**:
- [SQL Query Optimization](https://learn.microsoft.com/en-us/sql/relational-databases/performance/query-performance-tuning)
- [MongoDB Aggregation](https://docs.mongodb.com/manual/aggregation/)
- [PostgreSQL Query Optimization](https://www.postgresql.org/docs/current/performance-tips.html)

By recognizing and avoiding these antipatterns, developers can design more robust, performant, and maintainable database systems. Regular code reviews, performance monitoring, and staying updated with best practices are essential for long-term database health.