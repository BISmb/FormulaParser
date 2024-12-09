using System.Linq.Expressions;

namespace Formula.Expressions.Math;

// public class SumExpression : Expression
// {
//     public override Type Type => typeof(double);
//     public override ExpressionType NodeType => ExpressionType.Extension;
//     public override string ToString() => $"[]";
//
//     public override bool CanReduce => true;
//
//     private readonly IReadOnlyList<Expression> Variables;
//     
//     public SumExpression(IReadOnlyList<Expression> arrayExpression)
//     {
//         Variables = arrayExpression;
//     }
//
//     public override Expression Reduce()
//     {
//         // Parameters and variables
//         var counter = Expression.Parameter(typeof(int), "counter");
//         var sum = Expression.Parameter(typeof(int), "sum");
//         var loopLabel = Expression.Label("Loop");
//         
//         // Body of the loop
//         var loopBody = Expression.Block(
//             // Increment sum: sum += counter
//             Expression.AddAssign(sum, counter),
//
//             var accessExpression = Expression.ArrayAccess(Variables.ToArray(), counter);
//             
//             Expression.AddAssign(sum, Variables.ElementAt(counter)),
//             // 1 + 2 + 3
//
//             // Increment counter: counter++
//             Expression.PreIncrementAssign(counter),
//
//             // Break if condition is met: if (counter > 10) break;
//             Expression.IfThen(
//                 Expression.GreaterThan(counter, Expression.Constant(Variables.Count)),
//                 Expression.Break(loopLabel)
//             )
//         );
//
//         // Loop construct
//         var loop = Expression.Loop(loopBody, loopLabel);
//
//         // Create a block that initializes variables, runs the loop, and returns the result
//         var block = Expression.Block(
//             new[] { counter, sum }, // Declare variables
//             Expression.Assign(counter, Expression.Constant(0)), // counter = 0
//             Expression.Assign(sum, Expression.Constant(0)),     // sum = 0
//             loop,                                               // Execute the loop
//             sum                                                 // Return the sum
//         );
//         
//         // return block
//     }
//
//     protected override Expression VisitChildren(ExpressionVisitor visitor)
//     {
//         Expression[] visitedChildren = Variables
//             .Where(e => e.Type == typeof(double))
//             .Select(visitor.Visit)
//             .ToArray();
//
//         if (visitedChildren.SequenceEqual(Variables))
//         {
//             return this;
//         }
//         
//         return new SumExpression(visitedChildren);
//     }
// }

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

        var flattenedBlockExpressions = reducedExpressions
            .OfType<BlockExpression>()
            .SelectMany(b => b.Expressions);
        
        reducedExpressions.AddRange(flattenedBlockExpressions);
        
        if (!reducedExpressions.Any())
            throw new InvalidOperationException("No expressions to reduce.");

        Expression sum = reducedExpressions.First();
        foreach (var expr in reducedExpressions.Skip(1))
        {
            sum = Add(sum, expr);
        }

        return sum;
    }

    // Override NodeType to return a custom type for the expression
    public override Type Type => typeof(int);
    public override ExpressionType NodeType => ExpressionType.Add;
    public override bool CanReduce => true;
}