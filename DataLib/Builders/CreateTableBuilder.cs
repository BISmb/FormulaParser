using System.Linq.Expressions;
using DataLib.Abstraction.Interfaces;
using DataLib.Expressions;

namespace DataLib.Builders;

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
        Expression? expression = new CreateTableExpression(_name);
        
        // if idempotent is enabled
        expression = new IdempotentCreateTableExpression((CreateTableExpression)expression);
        
        // Expression expression = new CreateTableExpression(_name);
        
        foreach (var visitor in visitors)
        {
            expression = visitor.Visit(expression);
        }

        return expression;
    }
}