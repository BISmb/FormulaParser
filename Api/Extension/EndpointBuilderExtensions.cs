namespace Api.Extension;

public static partial class EndpointBuilderExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        MapSourceGeneratedEndpoints(app);
    }
    
    static partial void MapSourceGeneratedEndpoints(IEndpointRouteBuilder routeBuilder);
}

// public static partial class EndpointBuilderExtensions
// {
//     static partial void MapSourceGeneratedEndpoints(IEndpointRouteBuilder routeBuilder)
//     {
//         throw new System.NotImplementedException();
//     }
// }