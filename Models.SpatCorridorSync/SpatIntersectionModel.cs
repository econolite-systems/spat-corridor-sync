using Econolite.Ode.Persistence.Common.Entities;

namespace Econolite.Ode.Models.SpatCorridorSync;

public class SpatIntersectionModel : GuidIndexedEntityBase
{
    public long? ClarityId { get; set; }
        
    public int? SpatId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ControllerType { get; set; } = string.Empty;

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }
        
    public bool IsDeleted { get; set; }
}