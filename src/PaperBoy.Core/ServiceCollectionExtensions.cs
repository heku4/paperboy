using Microsoft.Extensions.DependencyInjection;

namespace PaperBoy.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.AddSingleton<IGrpcCallProvider, GrpcCallProvider>();
    }
}