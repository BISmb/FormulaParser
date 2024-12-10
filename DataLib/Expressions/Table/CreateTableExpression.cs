using System.Linq.Expressions;
using System.Text;
using DataLib.Models;
using DataLib.Visitor;

namespace DataLib.Expressions;

public abstract class DdlExpression : Expression
{
    public override bool CanReduce => false;
}

public class CreateTableExpression
    : Expression
{
    public string TableName { get; }
    public string? TableNameQuoted { get; }

    internal CreateTableExpression(CreateTableOptions options)
    {
        TableName = options.TableName;
    }

    public CreateTableExpression(string tableName, string? quotedTable = null)
    {
        TableName = tableName;
        TableNameQuoted = quotedTable ?? tableName;
    }

    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is QuoterVisitor qVisitor)
        {
            string quotedTableName = $"{qVisitor.OpenQuote}{TableName}{qVisitor.CloseQuote}";
            return new CreateTableExpression(TableName, quotedTableName);
        }

        // if (visitor is IdempotenceVisitor idempotenceVisitor)
        // {
        //     return idempotenceVisitor.Visit(TableName);
        // }

        return this;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin(' ', "CREATE", "TABLE", TableNameQuoted);
        string statement = sb.ToString();
        
        return statement;
    }
}

public class IdempotentCreateTableExpression(Expression ddlExpression)
    : Expression
{
    private Func<Expression, string>? _convertToIdempotentFunc;
    private Expression Expression = ddlExpression;
    
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is IdempotenceVisitor idempotenceVisitor)
        {
            _convertToIdempotentFunc = idempotenceVisitor.IdempotentTransform;
            return this;
        }
        
        Expression = visitor.Visit(ddlExpression);
        
        // CreateTableExpression visitedCreateTableExpression = _ddlExpression.Accept(visitor) as CreateTableExpression
        //                                                      ?? throw new Exception($"Can't create CreateTableExpression");
        //

        return this; //base.Accept(visitor);
    }

    public override string ToString()
    {
        string idempotentScript = _convertToIdempotentFunc?.Invoke(Expression) ?? throw new InvalidOperationException("No function given to apply idempotent");
        return idempotentScript;
    }
}