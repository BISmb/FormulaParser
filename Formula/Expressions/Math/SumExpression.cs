using System.Linq.Expressions;

namespace Formula.Expressions.Math;

public class SumExpression : Expression
{
    public List<Expression> Expressions { get; } = new();

    // Constructor to initialize with multiple parameters and property paths
    public SumExpression(IEnumerable<Expression> expressions)
    {
        Expressions.AddRange(expressions);
    }

    // Build the sum expression dynamically
    public Expression ToExpression()
    {
        if (!Expressions.Any())
            throw new InvalidOperationException("No expressions to sum.");

        // Start with the first expression
        Expression sum = Expressions.First();
        foreach (var expr in Expressions.Skip(1))
        {
            sum = Expression.Add(sum, expr);
        }

        return sum;
    }

    // Convert to a lambda expression
    public LambdaExpression ToLambda(IEnumerable<ParameterExpression> parameters)
    {
        return Expression.Lambda(ToExpression(), parameters);
    }

    // Implement Reduce to reduce the SumExpression into a simpler expression
    public override Expression Reduce()
    {
        var reducedExpressions = Expressions
            .Select(p => p.CanReduce ? p.Reduce() : p)
            .ToList();
        
        if (!reducedExpressions.Any())
            throw new InvalidOperationException("No expressions to reduce.");

        Expression sum = Expression.Constant(0D);
        foreach (var expr in reducedExpressions)
        {   
            if (expr is BlockExpression block)
            {
                foreach (var blockExpression in block.Expressions)
                {
                    sum = Add(sum, blockExpression);
                }
            }
            else if (expr is NewArrayExpression newArrayExpression)
            {
                foreach (var blockExpression in newArrayExpression.Expressions)
                {
                    sum = Add(sum, blockExpression);
                } 
            }
            else
            {
                sum = Add(sum, expr);
            }
        }

        return sum;
    }

    // Override NodeType to return a custom type for the expression
    public override Type Type => typeof(double);
    public override ExpressionType NodeType => ExpressionType.Extension;
    public override bool CanReduce => true;
}