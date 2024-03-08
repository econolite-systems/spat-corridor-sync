using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Models.SpatCorridorSync;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Messaging.SpatCorridorSync.Extensions;

public static class Defined
{
    public static IServiceCollection AddCorridorConsumer(this IServiceCollection services, Action<CorridorConsumerOptions> options) => services
        .Configure<CorridorConsumerOptions>(_ => options(_))
        .AddMessaging()
        .AddTransient<IPayloadSpecialist<IEnumerable<CorridorsSyncModel>>, JsonPayloadSpecialist<IEnumerable<CorridorsSyncModel>>>()
        .AddTransient<IConsumeResultFactory<Guid, IEnumerable<CorridorsSyncModel>>, ConsumeResultFactory<IEnumerable<CorridorsSyncModel>>>()
        .AddTransient<IConsumer<Guid, IEnumerable<CorridorsSyncModel>>, Consumer<Guid, IEnumerable<CorridorsSyncModel>>>()
        .AddTransient<ICorridorConsumer, CorridorConsumer>();
    
    public static IServiceCollection AddIntersectionConsumer(this IServiceCollection services, Action<IntersectionConsumerOptions> options) => services
        .Configure<IntersectionConsumerOptions>(_ => options(_))
        .AddMessaging()
        .AddTransient<IPayloadSpecialist<IEnumerable<SpatIntersectionModel>>, JsonPayloadSpecialist<IEnumerable<SpatIntersectionModel>>>()
        .AddTransient<IConsumeResultFactory<Guid, IEnumerable<SpatIntersectionModel>>, ConsumeResultFactory<IEnumerable<SpatIntersectionModel>>>()
        .AddTransient<IConsumer<Guid, IEnumerable<SpatIntersectionModel>>, Consumer<Guid, IEnumerable<SpatIntersectionModel>>>()
        .AddTransient<IIntersectionConsumer, IntersectionConsumer>();
}