using System.Data;
using System.Linq.Expressions;
using DataLib.Abstraction.Interfaces;
using DataLib.Expressions;
using DataLib.Models;

namespace DataLib.Builders;

public class NewTableBuilder(ISchemaGenerationSettings settings) :
    ICreateTableBuilder,
    ICreateTableWithNameBuilder
{
    private CreateTableOptions _creationOptions;

    internal NewTableBuilder(ISchemaGenerationSettings generationSettings, CreateTableOptions options)
        : this(generationSettings)
    {
        _creationOptions = options;
    }
    
    public ICreateTableWithNameBuilder WithName(string name)
    {
        _creationOptions = _creationOptions with
        {
            TableName = name,
        };

        return this;
    }
    
    public ICreateTableWithNameBuilder WithPrimaryKeyInformation(string columnName, DbType dbType)
    {
        _creationOptions = _creationOptions with
        {
            PrimaryKeyColumnName = columnName,
            PrimaryKeyType = dbType,
        };
        
        return this;
    }

    public Expression BuildExpression(params ExpressionVisitor[] visitors)
    {   
        Expression? expression = new CreateTableExpression(_creationOptions);
        
        if (settings.ProduceIdempotentSql)
        {
            expression = new IdempotentCreateTableExpression((CreateTableExpression)expression);
        }
        
        foreach (var visitor in visitors)
        {
            expression = visitor.Visit(expression);
        }

        return expression;
    }
}