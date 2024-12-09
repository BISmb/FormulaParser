using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DataLib.Abstraction.Interfaces;
using DataLib.Builders;
using DataLib.Expressions;
using DataLib.Visitor;

namespace DataLib.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        IDataSchemaBuilder schemaBuilder = new DataSchemaBuilder();
            
        schemaBuilder
            .NewTable("table_a");
        
        schemaBuilder
            .NewTable("table_b");

        var visitor = new MsSqlQuoter();
        var visitor2 = new MsSqlServerIdempotentTransformer();
        string sql = schemaBuilder.GetStatement(visitor, visitor2);
    }
    
    public class PostgresQuoter : HomogonousQuoterVisitor
    {
        public override char QuoteChar => '\"';
    }

    public class PostgresIdempotentTransformer : IdempotenceVisitor
    {
        public override string IdempotentTransform(CreateTableExpression createTableExpression)
        {
            return $"CREATE TABLE IF NOT EXISTS {createTableExpression.TableName}";
        }
    }
    
    public class MsSqlQuoter : QuoterVisitor
    {
        public override char OpenQuote => '[';
        public override char CloseQuote => ']';
    }

    public class MsSqlServerIdempotentTransformer : IdempotenceVisitor
    {
        public override string IdempotentTransform(CreateTableExpression createTableExpression)
        {
            return $"""
                    IF NOT EXISTS (
                         SELECT 1 
                         FROM INFORMATION_SCHEMA.TABLES 
                         WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{createTableExpression.TableName}')
                    BEGIN
                        {createTableExpression}
                    END
                    """;
        }
    }
}