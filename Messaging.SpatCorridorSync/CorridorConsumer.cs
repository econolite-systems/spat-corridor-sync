using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.SpatCorridorSync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Messaging.SpatCorridorSync;

public class CorridorConsumer : ICorridorConsumer
{
    private readonly IConsumer<Guid, IEnumerable<CorridorsSyncModel>> _consumer;
    private readonly string[] _wantedtypes;

    public CorridorConsumer(IConfiguration configuration, IConsumer<Guid, IEnumerable<CorridorsSyncModel>> consumer, IOptions<CorridorConsumerOptions> options)
    {
        _consumer = consumer;
        _wantedtypes = new string[]
        {
            "CorridorsSyncModel[]",
        };
        _consumer.Subscribe(configuration[options.Value.ConfigTopic]);
    }

    public void Complete(ConsumeResult consumeResult) => _consumer.Complete(consumeResult);

    public (ConsumeResult ConsumeResult, IEnumerable<CorridorsSyncModel> Corridors) Consume(CancellationToken cancellationToken)
    {
        var consumeresult = _consumer.Consume(_ => _wantedtypes.Contains(_), cancellationToken);
        return (consumeresult, consumeresult.ToObject<IEnumerable<CorridorsSyncModel>>());
    }
}