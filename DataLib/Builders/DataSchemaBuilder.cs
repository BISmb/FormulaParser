using System.Linq.Expressions;
using System.Text;
using DataLib.Abstraction.Interfaces;
using DataLib.Expressions;
using DataLib.Visitor;

namespace DataLib.Builders;

public class DataSchemaBuilder : IDataSchemaBuilder
{
    private IList<ISqlBuilder> _builders = new List<ISqlBuilder>();
    
    public ICreateTableBuilder NewTable(string tableName)
    {
        var tableBuilder = new CreateTableBuilder();
        tableBuilder.WithName(tableName);
        _builders.Add(tableBuilder);
        
        return tableBuilder;
    }

    public Expression BuildExpression(params ExpressionVisitor[] visitors)
    {
        var childExpressions = _builders
            .Select(b => b.BuildExpression(visitors))
            .ToArray()
            .AsReadOnly();
        
        return new DefineSchemaExpression(
            childExpressions
        );
    }

    public string GetStatement(params ExpressionVisitor[] visitors)
    {
        // idempotent visitors are applied last
        visitors = visitors
            .OrderBy(v => v is IdempotenceVisitor)
            .ThenBy(v => v)
            .ToArray();
        
        var schemaExpression = BuildExpression(visitors);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin("", $"{schemaExpression}");
        return sb.ToString();
    }
}