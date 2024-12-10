using System.Data;

namespace DataLib.Abstraction.Interfaces;

public interface ICreateTableBuilder : ISqlBuilder
{
    ICreateTableWithNameBuilder WithName(string name);
}

public interface ICreateTableWithNameBuilder : ISqlBuilder
{
    ICreateTableWithNameBuilder WithPrimaryKeyInformation(string name, DbType dbType);
}