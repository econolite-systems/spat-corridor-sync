namespace Econolite.Ode.Models.SpatCorridorSync;

public class CorridorsSyncModel
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
        
    public bool IsDeleted { get; set; }
        
    public long[] Intersections { get; set; } = Array.Empty<long>();
}