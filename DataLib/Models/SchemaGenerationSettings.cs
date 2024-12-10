using DataLib.Abstraction.Interfaces;

namespace DataLib.Services;

public struct DefaultSchemaGenerationSettings
    : ISchemaGenerationSettings
{
    public bool ProduceIdempotentSql { get; }

    public DefaultSchemaGenerationSettings(bool produceIdempotentSql = false)
    {
        ProduceIdempotentSql = produceIdempotentSql;
    }
}