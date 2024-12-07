using System.Linq.Expressions;
using Formula.Expressions;
using Formula.Models;

namespace Formula.Visitors;

public class GridExpressionVisitor(IGrid grid) : ExpressionVisitor
{
    private IGrid _grid { get; } = grid;
    
    public override Expression? Visit(Expression? node)
    {
        if (node is not GridCellExpression cellReferenceExpression)
        {
            return base.Visit(node);
        }
        
        var referenceCellValue = _grid.GetValueForCell(cellReferenceExpression.CellReference);

        // todo: return constant for now (no cascading referenced functions)
        return Expression.Constant(referenceCellValue);
    }
}