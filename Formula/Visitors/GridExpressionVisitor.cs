using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Formula.Expressions;
using Formula.Expressions.Grid;
using Formula.Expressions.Math;
using Formula.Models;

namespace Formula.Visitors;

public class GridExpressionVisitor(IGrid grid) : ExpressionVisitor
{
    private IGrid _grid { get; } = grid;
    public IGrid Grid => _grid;

    [return: NotNullIfNotNull("node")]
    public override Expression? Visit(Expression? node)
    {
        if (node is null)
        {
            return Expression.Empty();
        }

        // replace cell reference expression
        if (node is CellReferenceExpression cellReferenceExpression)
        {
            object cellValue = _grid.GetValueForCell(cellReferenceExpression.CellReference);
            return Expression.Convert(Expression.Constant(cellValue), typeof(double));
        }

        // if (node is SumExpression sumExpression)
        // {
        //     if (!sumExpression.Expressions.Any())
        //         throw new InvalidOperationException("No expressions to reduce.");
        //
        //     // Start with the first expression and add others to it
        //     Expression sum = Expression.Add(Expression.Constant(0D), sumExpression.Expressions.First());
        //     foreach (var expr in sumExpression.Expressions.Skip(1))
        //     {
        //         sum = Expression.Add(sum, expr);
        //     }
        //
        //     // check to see if sum.Type is same as sumExpression.Type
        //     Debugger.Break();
        // }
        
        return base.Visit(node);
    }
}