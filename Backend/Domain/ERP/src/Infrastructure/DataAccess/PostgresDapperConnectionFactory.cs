using System.Data;
using B2Connect.ERP.Infrastructure.DataAccess;
using Npgsql;

namespace B2Connect.ERP.Infrastructure.DataAccess;

/// <summary>
/// PostgreSQL implementation of the Dapper connection factory.
/// Uses the same connection string as EF Core for consistency.
/// </summary>
public class PostgresDapperConnectionFactory : IDapperConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresDapperConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    public PostgresDapperConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Creates a new PostgreSQL connection for Dapper operations.
    /// </summary>
    /// <returns>A new NpgsqlConnection instance.</returns>
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
