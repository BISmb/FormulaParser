using System.Linq.Expressions;

namespace DataLib.Abstraction.Interfaces;

public interface IDataSchemaBuilder : ISqlBuilder
{
    ICreateTableWithNameBuilder NewTable(string tableName);
    string GetStatement(params ExpressionVisitor[] visitors);
}