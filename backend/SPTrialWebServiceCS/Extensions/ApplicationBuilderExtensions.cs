using System.Reflection;

namespace SwissPension.Trial.Tree.Extensions;

public static class ApplicationBuilderExtensions
{
    private const string ServiceNamespace = ".Services";

    public static IApplicationBuilder MapGrpcServices(this IApplicationBuilder app)
    {
        var baseNamespace = Assembly.GetExecutingAssembly().GetName().Name;
        var serviceTypeNamespace = $"{baseNamespace}{ServiceNamespace}";

        var serviceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.Namespace?.StartsWith(serviceTypeNamespace) == true);

        serviceTypes = serviceTypes.Where(x => !x.IsNested);

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