using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DataLib.Visitor;

namespace DataLib.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var schemaBuilder = new DataSchemaBuilder();
            
        schemaBuilder
            .NewTable("table_a");
        
        schemaBuilder
            .NewTable("table_b");

        var visitor = new PostgresVisitor();
        string sql = schemaBuilder.GetStatement(visitor);
    }
    
    public class PostgresVisitor : QuoterVisitor
    {
        public override char? QuoteChar => '\"';
    }
}