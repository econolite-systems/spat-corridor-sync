using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.SpatCorridorSync;

namespace Econolite.Ode.Messaging.SpatCorridorSync;

public interface ICorridorConsumer
{
    void Complete(ConsumeResult consumeResult);
    (ConsumeResult ConsumeResult, IEnumerable<CorridorsSyncModel> Corridors) Consume(CancellationToken cancellationToken);
}