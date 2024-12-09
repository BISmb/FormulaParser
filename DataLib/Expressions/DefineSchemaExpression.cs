using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Text;

namespace DataLib.Expressions;

public class DefineSchemaExpression(params IEnumerable<Expression> expressions) 
    : Expression
{
    public FrozenSet<Expression> Expressions { get; } = expressions.ToFrozenSet();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        
        string[] statments = Expressions
            .Select(expr => expr.ToString())
            .ToArray();
        
        sb.AppendJoin("; ", statments);
        return sb.ToString();
    }
}