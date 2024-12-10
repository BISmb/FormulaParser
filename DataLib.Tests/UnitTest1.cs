using System.Data;
using DataLib.Abstraction.Interfaces;
using DataLib.Builders;
using DataLib.Expressions;
using DataLib.Services;
using DataLib.Visitor;
using FluentAssertions;

namespace DataLib.Tests;

public class UnitTest1
{
    private string _sql;

    public UnitTest1()
    {
        IDataSchemaBuilder schemaBuilder = new DataSchemaBuilder(new DefaultSchemaGenerationSettings(false));
            
        schemaBuilder
            .NewTable("table_a");
        
        schemaBuilder
            .NewTable("table_b")
            .WithPrimaryKeyInformation("id", DbType.Guid);

        var visitor = new MsSqlQuoter();
        var visitor2 = new MsSqlServerIdempotentTransformer();
        
        _sql = schemaBuilder.GetStatement(visitor, visitor2);
    }
    
    [Fact]
    public void Should_Have_CreateTableA_Expression()
    {
        _sql
            .Should()
            .ContainEquivalentOf("create table [table_a];");
    }
    
    [Fact]
    public void Should_Have_CreateTableB_Expression()
    {
        _sql
            .Should()
            .ContainEquivalentOf("create table [table_b];");
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