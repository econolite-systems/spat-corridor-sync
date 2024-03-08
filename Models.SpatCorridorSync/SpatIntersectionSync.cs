using Econolite.Ode.Persistence.Common.Entities;

namespace Econolite.Ode.Models.SpatCorridorSync;

public class SpatIntersectionSync : GuidIndexedEntityBase
{
    public Guid TenantId { get; set; }
    
    public string ExternalId { get; set; } = string.Empty;
        
    public int? SpatId { get; set; }

    public string Name { get; set; } = string.Empty;
        
    public string Description { get; set; } = string.Empty;

    public string ControllerType { get; set; } = string.Empty;

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }
        
    public bool IsDeleted { get; set; }
}

public static class SpatIntersectionExtensions
{
    public static IEnumerable<SpatIntersectionSync> ToDb(this IEnumerable<SpatIntersectionModel> models, Guid tenantId)
    {
        return models.Select(m => m.ToDb(tenantId));
    }
    
    public static SpatIntersectionSync ToDb(this SpatIntersectionModel model, Guid tenantId)
    {
        var result = new SpatIntersectionSync
        {
            Id = model.Id,
            TenantId = tenantId,
            ExternalId = model.ClarityId?.ToString() ?? string.Empty,
            ControllerType = model.ControllerType,
            Description = model.Description,
            IsDeleted = model.IsDeleted,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            Name = model.Name,
            SpatId = model.SpatId,
        };
        
        return result;
    }

    public static bool ShouldSync(this SpatIntersectionSync current, SpatIntersectionModel model)
    {
        return current.IsDifferent(model);
    }
    
    public static bool IsDifferent(this SpatIntersectionSync current, SpatIntersectionModel model)
    {
        if (current.Description != model.Description ||
            current.Latitude != model.Latitude ||
            current.Longitude != model.Longitude ||
            current.Name != model.Name ||
            current.ControllerType != model.ControllerType ||
            current.SpatId != model.SpatId ||
            current.IsDeleted != model.IsDeleted)
        {
            return true;
        }

        return false;
    }
}