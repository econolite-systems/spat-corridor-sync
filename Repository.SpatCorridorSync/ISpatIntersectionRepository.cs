using Econolite.Ode.Models.SpatCorridorSync;
using Econolite.Ode.Persistence.Common.Repository;

namespace Repository.SpatCorridorSync;

public interface ISpatIntersectionRepository: IRepository<SpatIntersectionSync, Guid>
{
    Task<ICollection<SpatIntersectionSync>> GetByTenantAsync(Guid id);
}