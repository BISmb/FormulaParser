namespace DataLib.Abstraction.Interfaces;

public interface ISchemaGenerationSettings
{
    bool ProduceIdempotentSql { get; }
}