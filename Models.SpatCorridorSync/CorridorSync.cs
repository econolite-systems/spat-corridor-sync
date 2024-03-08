using Econolite.Ode.Persistence.Common.Entities;

namespace Econolite.Ode.Models.SpatCorridorSync;

public class CorridorSync : GuidIndexedEntityBase
{
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public long[] Intersections { get; set; } = new long[] {};
    public Guid TenantId { get; set; }
}

public static class CorridorExtensions
{
    public static IEnumerable<CorridorSync> ToDb(this IEnumerable<CorridorsSyncModel> models, Guid tenantId)
    {
        return models.Select(m => m.ToDb(Guid.NewGuid(), tenantId));
    }
    
    public static CorridorSync ToDb(this CorridorsSyncModel model, Guid id, Guid tenantId)
    {
        var result = new CorridorSync()
        {
            Id = id,
            TenantId = tenantId,
            ExternalId = model.Id.ToString(),
            IsDeleted = model.IsDeleted,
            Name = model.Name,
            Intersections = model.Intersections
        };
        
        return result;
    }

    public static bool ShouldSync(this CorridorSync current, CorridorsSyncModel model)
    {
        return current.IsDifferent(model);
    }
    
    public static bool IsDifferent(this CorridorSync current, CorridorsSyncModel model)
    {
        if (
            (current.Name != model.Name ||
            current.IsDeleted != model.IsDeleted ||
            !current.Intersections.SequenceEqual(model.Intersections)) &&
            ((current.Intersections.Length > 0 && model.Intersections.Length > 0) ||
             (current.Intersections.Length == 0 && model.Intersections.Length > 0) ||
             (current.Intersections.Length > 0 && model.Intersections.Length == 0))
        )
        {
            return true;
        }

        return false;
    }
}