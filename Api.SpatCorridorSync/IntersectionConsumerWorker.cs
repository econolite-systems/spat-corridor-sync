using Econolite.Ode.Api.SpatCorridorSync.Services;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.SpatCorridorSync;
using Econolite.Ode.Models.SpatCorridorSync;
using Repository.SpatCorridorSync;

namespace Econolite.Ode.Api.SpatCorridorSync;

public class IntersectionConsumerWorker : BackgroundService
{
    private readonly ILogger<IntersectionConsumerWorker> _logger;
    private readonly IIntersectionConsumer _consumer;
    private readonly SyncService _syncService;
    private readonly ISpatIntersectionRepository _repository;
    private readonly Guid _tenantId;

    public IntersectionConsumerWorker(
        ILogger<IntersectionConsumerWorker> logger,
        IConfiguration config,
        IIntersectionConsumer consumer,
        IServiceProvider provider,
        SyncService syncService
    )
    {
        _logger = logger;
        _consumer = consumer;
        _syncService = syncService;
        _tenantId = config.GetValue<Guid>("Tenant");
        _repository = provider.CreateScope().ServiceProvider.GetRequiredService<ISpatIntersectionRepository>();
    }

    private async Task HandleMessage(IEnumerable<SpatIntersectionModel> message, Guid tenantId)
    {
        if (_tenantId != tenantId) return;
        
        var currentIntersections = await _repository.GetByTenantAsync(tenantId);
        //var sync = message.ToDb(tenantId);
        var toSync = new List<SpatIntersectionModel>();
        foreach (var item in message)
        {
            var current = currentIntersections.SingleOrDefault(c => c.Id == item.Id);
            if (current != null)
            {
                if (current.ShouldSync(item))
                {
                    toSync.Add(item);
                }
                _repository.Update(item.ToDb(tenantId));
            }
            else
            {
                if (!item.IsDeleted)
                {
                    toSync.Add(item);
                }
                
                _repository.Add(item.ToDb(tenantId));
            }
        }

        var result = await _repository.DbContext.SaveChangesAsync();
        if (toSync.Any())
        {
            var send = toSync.ToEntitySync(Guid.NewGuid(), tenantId);
            var success = await _syncService.SendToEntitySync(send);
            _logger.LogTrace("Save: {result}, sent to entity: {success}", result, success);
        }
        _logger.LogInformation("Handle Intersection message: {}", toSync);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);

                    try
                    {
                        var consumeResult = result.ConsumeResult as ConsumeResult<Guid, IEnumerable<SpatIntersectionModel>>;
                        await HandleMessage(result.Intersections, consumeResult?.Key ?? throw new InvalidCastException($"Consume result is not of type: ConsumeResult<Guid, IEnumerable<SpatIntersectionModel>>, instead received: {result.ConsumeResult.GetType()}"));
                        _consumer.Complete(result.ConsumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Uncaught exception while processing message: {}", result.ConsumeResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Intersection consumer worker stopping");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Intersection consumer worker stopping due to uncaught exception");
            }
        }, stoppingToken);
    }
}