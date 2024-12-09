using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DataLib.Expressions;

namespace DataLib.Visitor;

/// <summary>
/// Transforms a statement to be idempotent (patching versus apply diffs)
/// </summary>
public abstract class IdempotenceVisitor : ExpressionVisitor
{
    internal string IdempotentTransform(Expression expression)
    {
        if (expression is CreateTableExpression createTableExpression)
        {
            return IdempotentTransform(createTableExpression);
        }
        
        return expression.ToString();
    }
    
    public abstract string IdempotentTransform(CreateTableExpression createTableExpression);
}