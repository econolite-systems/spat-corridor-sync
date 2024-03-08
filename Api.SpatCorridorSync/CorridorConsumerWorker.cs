using Econolite.Ode.Api.SpatCorridorSync.Services;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.SpatCorridorSync;
using Econolite.Ode.Models.SpatCorridorSync;
using Repository.SpatCorridorSync;

namespace Econolite.Ode.Api.SpatCorridorSync;

public class CorridorConsumerWorker : BackgroundService
{
    private readonly ILogger<CorridorConsumerWorker> _logger;
    private readonly ICorridorConsumer _consumer;
    private readonly SyncService _syncService;
    private readonly ISpatCorridorRepository _repository;
    private readonly Guid _tenantId;

    public CorridorConsumerWorker(
        ILogger<CorridorConsumerWorker> logger,
        IConfiguration config,
        ICorridorConsumer consumer,
        IServiceProvider provider,
        SyncService syncService
    )
    {
        _logger = logger;
        _consumer = consumer;
        _syncService = syncService;
        _tenantId = config.GetValue<Guid>("Tenant");
        _repository = provider.CreateScope().ServiceProvider.GetRequiredService<ISpatCorridorRepository>();
    }

    private async Task HandleMessage(IEnumerable<CorridorsSyncModel> message, Guid tenantId)
    {
        if (_tenantId != tenantId) return;
        
        var currentCorridors = await _repository.GetByTenantAsync(tenantId);
        //var sync = message.ToDb(tenantId);
        var toSync = new List<CorridorsSyncModel>();
        foreach (var item in message)
        {
            var current = currentCorridors.SingleOrDefault(c => c.ExternalId == item.Id.ToString());
            if (current != null)
            {
                if (current.ShouldSync(item))
                {
                    toSync.Add(item);
                }
                _repository.Update(item.ToDb(current.Id, tenantId));
            }
            else
            {
                if (!item.IsDeleted && item.Intersections.Any())
                {
                    toSync.Add(item);
                }
                
                _repository.Add(item.ToDb(Guid.NewGuid(), tenantId));
            }
        }

        var result = await _repository.DbContext.SaveChangesAsync();
        if (toSync.Any())
        {
            var send = toSync.ToEntitySync(Guid.NewGuid(), tenantId);
            var success = await _syncService.SendToEntitySync(send);
            _logger.LogTrace("Save: {result}, sent to entity: {success}", result, success);
        }
        
        _logger.LogInformation("Handle Corridors message: {}", toSync);
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
                        var consumeResult = result.ConsumeResult as ConsumeResult<Guid, IEnumerable<CorridorsSyncModel>>;
                        await HandleMessage(result.Corridors, consumeResult?.TenantId ?? throw new InvalidCastException($"Consume result is not of type: ConsumeResult<Guid, IEnumerable<CorridorsSyncModel>>, instead received: {result.ConsumeResult.GetType()}"));
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
                _logger.LogInformation("Corridor consumer worker stopping");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Corridor consumer worker stopping due to uncaught exception");
            }
        }, stoppingToken);
    }
}