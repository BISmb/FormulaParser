using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApiLib.Abstractions.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

// namespace ApiLib.Gen
// {
//     [Generator]
//     public class Class1
//     {
//     }
// }

[Generator]
public class MySourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a callback to run during initialization
        // Useful for diagnostics or debugging (optional)
    }

    public void Execute(GeneratorExecutionContext context)
    {
        const string interfaceName = "IEndpoint";
        
        Compilation compilation = context.Compilation;
        INamedTypeSymbol interfaceSymbol = compilation.GetTypeByMetadataName(interfaceName);

        if (interfaceSymbol is null)
        {
            // Interface not found
            return;
        }

        // Get all types in the compilation
        var allTypes = GetAllTypes(compilation.GlobalNamespace);

        // Find types that implement the interface
        var typesImplementingInterface = allTypes
            .Where(type => type.TypeKind == TypeKind.Class && type.AllInterfaces.Contains(interfaceSymbol))
            .ToList();
        
        StringBuilder builder = new StringBuilder();
        foreach (var type in typesImplementingInterface)
        {
            var attr = type
                .GetAttributes()
                .First(a => a.AttributeClass?.ToDisplayString() == "ApiLib.Abstractions.Attributes.GetAttribute");
            
            foreach (var member in attr.AttributeClass.GetMembers())
            {
                if (member is IPropertySymbol property && property.Name == nameof(GetAttribute.Method))
                {
                    compilation.
                    
                    Console.WriteLine();
                    //var defaultValue = property.Value; // Get the default value
                }
            }
            
            // var path = attr.ConstructorArguments
            //     .FirstOrDefault(kv => kv.Key == nameof(GetAttribute.PathTemplate))
            //     .Value.Value?.ToString();

            string path = "";
            
            var method = attr.NamedArguments
                .FirstOrDefault(kv => kv.Key == nameof(GetAttribute.Method))
                .Value.Value?.ToString();
            
            string[] methods = [method];
            string methodStr = string.Join(",", methods.Select(method => $"\"{method}\""));
            
            string endPointBoilerplate =
                $$"""
                routeBuilder.MapMethods("{{path}}", [{{methodStr}}], (IServiceProvider sp, CancellationToken ct = default) =>
                {
                    IEndpoint endpoint = ActivatorUtilities.CreateInstance(sp, typeof(MyHelloEndpoint)) as IEndpoint
                        ?? throw new InvalidOperationException();
                    
                    return endpoint.HandleAsync(ct);
                });
                """;

            builder.AppendLine(endPointBoilerplate);
        }
        
        string source =
            $$"""
            namespace Api.Extension;
            
            public static partial class EndpointBuilderExtensions
            {   
                static partial void MapSourceGeneratedEndpoints(IEndpointRouteBuilder routeBuilder)
                {
                    {{builder}}
                }
            }
            """;

        // Add the generated source to the compilation
        context.AddSource("HelloWorldEndpoint", SourceText.From(source, Encoding.UTF8));
    }
    
    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamespaceSymbol childNamespace)
            {
                foreach (var type in GetAllTypes(childNamespace))
                {
                    yield return type;
                }
            }
            else if (member is INamedTypeSymbol namedType)
            {
                yield return namedType;

                foreach (var nestedType in namedType.GetTypeMembers())
                {
                    yield return nestedType;
                }
            }
        }
    }
    
}