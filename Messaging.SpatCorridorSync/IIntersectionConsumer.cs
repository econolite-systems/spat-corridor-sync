using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.SpatCorridorSync;

namespace Econolite.Ode.Messaging.SpatCorridorSync;

public interface IIntersectionConsumer
{
    void Complete(ConsumeResult consumeResult);
    (ConsumeResult ConsumeResult, IEnumerable<SpatIntersectionModel> Intersections) Consume(CancellationToken cancellationToken);
}