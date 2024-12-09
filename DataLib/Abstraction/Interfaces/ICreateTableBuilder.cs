namespace DataLib.Abstraction.Interfaces;

public interface ICreateTableBuilder : ISqlBuilder
{
    ICreateTableBuilder WithName(string name);
}