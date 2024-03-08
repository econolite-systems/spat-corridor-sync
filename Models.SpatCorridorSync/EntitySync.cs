using Econolite.Ode.Persistence.Common.Entities;

namespace Econolite.Ode.Models.SpatCorridorSync;

public class EntitySync : GuidIndexedEntityBase
{
    public Guid ExternalSystemId = Guid.Empty;
    public IEnumerable<CorridorsSyncModel> Corridors { get; set; } = Array.Empty<CorridorsSyncModel>();
    public IEnumerable<SpatIntersectionModel> Intersections { get; set; } = Array.Empty<SpatIntersectionModel>();
}

public static class EntitySyncExtensions
{
    public static EntitySync ToEntitySync(this IEnumerable<CorridorsSyncModel> models, Guid id, Guid tenantId)
    {
        return new EntitySync()
        {
            Id = id,
            ExternalSystemId = tenantId,
            Corridors = models
        };
    }
    
    public static EntitySync ToEntitySync(this IEnumerable<SpatIntersectionModel> models, Guid id, Guid tenantId)
    {
        return new EntitySync()
        {
            Id = id,
            ExternalSystemId = tenantId,
            Intersections = models
        };
    }
}