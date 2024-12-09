using System.Linq.Expressions;

namespace DataLib.Visitor;

public abstract class QuoterVisitor : ExpressionVisitor
{
    public abstract char? QuoteChar { get; }

    // public virtual Expression? VisitCreateTable(CreateTableExpression node)
    // {
    //     return node;
    // }
}