using System.Linq.Expressions;

namespace DataLib.Visitor;

/// <summary>
/// A quoter visitor will quote table names, column names, and schema
/// </summary>
public abstract class QuoterVisitor : ExpressionVisitor
{
    public abstract char OpenQuote { get; }

    public abstract char CloseQuote { get; }
}

public class HomogonousQuoterVisitor : QuoterVisitor
{
    public virtual char QuoteChar => '\0';
    
    public override char OpenQuote => QuoteChar;
    public override char CloseQuote => QuoteChar;
}

