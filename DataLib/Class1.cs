using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Text;
using DataLib.Visitor;

namespace DataLib;

public interface ISqlBuilder
{
    public Expression BuildExpression(params ExpressionVisitor[] visitors);
}

public interface IDataSchemaBuilder : ISqlBuilder
{
    ICreateTableBuilder NewTable(string tableName);
}

public interface ICreateTableBuilder : ISqlBuilder
{
    ICreateTableBuilder WithName(string name);
}

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
        
        return new SchemaDefinitionExpression(
            childExpressions
        );
    }

    public string GetStatement(params ExpressionVisitor[] visitors)
    {
        var schemaExpression = BuildExpression(visitors);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin("", $"{schemaExpression}");
        return sb.ToString();
    }
}

public class CreateTableBuilder
    : ICreateTableBuilder
{
    private string _name { get; set; }

    public CreateTableBuilder()
    {
        
    }

    public CreateTableBuilder(CreateTableBuilder existingBuilder)
    {
        _name = existingBuilder._name;
    }
    
    public ICreateTableBuilder WithName(string name)
    {   
        _name = name;
        return new CreateTableBuilder(this);
    }

    public Expression BuildExpression(params ExpressionVisitor[] visitors)
    {
        Expression expression = new CreateTableExpression(_name);
        
        foreach (var visitor in visitors)
        {
            expression = visitor.Visit(expression);
        }

        return expression;
    }
}

public class SchemaDefinitionExpression(params IEnumerable<Expression> expressions) 
    : Expression
{
    public FrozenSet<Expression> Expressions { get; } = expressions.ToFrozenSet();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        
        string[] statments = Expressions
            .Select(expr => expr.ToString())
            .ToArray();
        
        sb.AppendJoin("; ", statments);
        return sb.ToString();
    }
}

public class CreateTableExpression(string tableName)
    : Expression
{
    private char? _quoteChar;
    public string TableName { get; private set; } = tableName;

    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is QuoterVisitor qVisitor)
        {
            TableName = $"{qVisitor.QuoteChar}{tableName}{qVisitor.QuoteChar}";
        }

        return this;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin(' ', "CREATE", "TABLE", TableName);
        return sb.ToString();
    }
}