using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace SwissPension.Trial.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    private const string ServiceNamespace = ".Services";

    public static IApplicationBuilder MapGrpcServices(this IApplicationBuilder app, string serviceNamespace = ServiceNamespace)
    {
        // Get the entry assembly (the assembly that contains the Main method)
        var assembly = Assembly.GetEntryAssembly()!;
        
        // Get the base namespace of the assembly
        var baseNamespace = assembly.GetName().Name;

        // Combine the base namespace with the service namespace
        var serviceTypeNamespace = $"{baseNamespace}{ServiceNamespace}";

        // Get all service types in the assembly
        var serviceTypes = assembly
            .GetTypes()
            .Where(x => x.Namespace?.StartsWith(serviceTypeNamespace) == true);

        // Filter out nested types
        // - we only want the top-level service types (gRPC services)
        serviceTypes = serviceTypes.Where(x => !x.IsNested);

        // Map each service type
        foreach (var serviceTypeToMap in serviceTypes)
        {
            var method = typeof(GrpcEndpointRouteBuilderExtensions)
                .GetMethod(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService))
                !.MakeGenericMethod(serviceTypeToMap);

            _ = method.Invoke(null, [app]);
        }

        return app;
    }
}