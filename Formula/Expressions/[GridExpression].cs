using System.Linq.Expressions;
using Formula.Visitors;

namespace Formula.Expressions;

public abstract class GridExpression : Expression
{
    protected abstract Expression? Accept(GridExpressionVisitor gridVisitor);
    
    protected sealed override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is GridExpressionVisitor gridVisitor)
        {
            return Accept(gridVisitor)
                ?? throw new Exception("A grid expression was not returned from the grid visitor");
        }
        
        return base.Accept(visitor);
    }
}