using System.Data;

namespace DataLib.Models;

internal struct CreateTableOptions
{
    public string TableName { get; init; }
    public string PrimaryKeyColumnName { get; init; }
    public DbType PrimaryKeyType { get; init; }
}