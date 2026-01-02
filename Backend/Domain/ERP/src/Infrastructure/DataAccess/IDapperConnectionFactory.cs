using System.Data;

namespace B2Connect.ERP.Infrastructure.DataAccess;

/// <summary>
/// Factory interface for creating database connections for Dapper operations.
/// Provides a clean abstraction over connection creation and management.
/// </summary>
public interface IDapperConnectionFactory
{
    /// <summary>
    /// Creates a new database connection for Dapper operations.
    /// The caller is responsible for properly disposing the connection.
    /// </summary>
    /// <returns>A new database connection instance.</returns>
    IDbConnection CreateConnection();
}
