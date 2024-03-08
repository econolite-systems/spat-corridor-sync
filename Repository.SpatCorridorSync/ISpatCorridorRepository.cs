using Econolite.Ode.Models.SpatCorridorSync;
using Econolite.Ode.Persistence.Common.Repository;

namespace Repository.SpatCorridorSync;

public interface ISpatCorridorRepository: IRepository<CorridorSync, Guid>
{
    Task<ICollection<CorridorSync>> GetByTenantAsync(Guid id);
}