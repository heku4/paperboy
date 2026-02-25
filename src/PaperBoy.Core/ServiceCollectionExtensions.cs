using Microsoft.Extensions.DependencyInjection;
using PaperBoy.Core.ProtoParsing;
using PaperBoy.Core.Unary;

namespace PaperBoy.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IProtoParser, ProtoParser>()
            .AddSingleton<JsonParser>()
            .AddSingleton<ProtocDescriptorExecutor>();

        return services.AddSingleton<IGrpcCallProvider, GrpcCallProvider>();
    }
}