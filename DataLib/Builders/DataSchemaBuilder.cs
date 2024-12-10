using System.Linq.Expressions;
using System.Text;
using DataLib.Abstraction.Interfaces;
using DataLib.Expressions;
using DataLib.Visitor;

namespace DataLib.Builders;

public class BuilderFactory(ISchemaGenerationSettings settings)
{
    public NewTableBuilder CreateTableBuilder()
    {
        return new NewTableBuilder(settings);
    }
}

public class DataSchemaBuilder : IDataSchemaBuilder
{
    private readonly IList<ISqlBuilder> _builders;
    private readonly BuilderFactory _builderFactory;

    public DataSchemaBuilder(ISchemaGenerationSettings settings)
    {
        _builders = new List<ISqlBuilder>();
        _builderFactory = new BuilderFactory(settings);
    }
    
    public ICreateTableWithNameBuilder NewTable(string tableName)
    {
        var tableBuilder = _builderFactory.CreateTableBuilder();
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