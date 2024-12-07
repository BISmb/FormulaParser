using System.Linq.Expressions;

namespace Formula.Evaluators;

public interface IFormulaEvaluator
{
    Expression FormulaToExpression(string formula);
    object EvaluateFormula(string formula, ExpressionVisitor? visitor = null);
}