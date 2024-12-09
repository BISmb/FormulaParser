using System.Linq.Expressions;

namespace DataLib.Abstraction.Interfaces;

public interface ISqlBuilder
{
    public Expression BuildExpression(params ExpressionVisitor[] visitors);
}