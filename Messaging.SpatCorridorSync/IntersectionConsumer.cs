using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.SpatCorridorSync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Messaging.SpatCorridorSync;

public class IntersectionConsumer : IIntersectionConsumer
{
    private readonly IConsumer<Guid, IEnumerable<SpatIntersectionModel>> _consumer;

    public IntersectionConsumer(IConfiguration configuration, IConsumer<Guid, IEnumerable<SpatIntersectionModel>> consumer, IOptions<IntersectionConsumerOptions> options)
    {
        _consumer = consumer;
        _consumer.Subscribe(configuration[options.Value.ConfigTopic]);
    }

    public void Complete(ConsumeResult consumeResult) => _consumer.Complete(consumeResult);

    public (ConsumeResult ConsumeResult, IEnumerable<SpatIntersectionModel> Intersections) Consume(CancellationToken cancellationToken)
    {
        var consumeresult = _consumer.Consume(_ => true, cancellationToken);
        return (consumeresult, consumeresult.ToObject<IEnumerable<SpatIntersectionModel>>());
    }
}