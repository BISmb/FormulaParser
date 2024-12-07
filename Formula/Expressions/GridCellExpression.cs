using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Formula.Visitors;

namespace Formula.Expressions;

// todo, this expression will need "reducing"
public class GridCellExpression(string cellReference) : GridExpression
{
    public override Type Type => typeof(GridCellExpression);
    public override bool CanReduce => true;
    public override string ToString() => $"{CellReference}";

    public readonly string CellReference = cellReference;
    
    protected override Expression Accept(GridExpressionVisitor gridVisitor)
    {
        return gridVisitor.Visit(this) 
               ?? throw new InvalidOperationException("A grid expression was used but no grid visitor was provided.");
    }
}