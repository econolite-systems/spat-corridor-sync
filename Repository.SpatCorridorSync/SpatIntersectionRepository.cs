using Econolite.Ode.Models.SpatCorridorSync;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Repository.SpatCorridorSync;

public class SpatIntersectionRepository: GuidDocumentRepositoryBase<SpatIntersectionSync>, ISpatIntersectionRepository
{
    public SpatIntersectionRepository(IMongoContext context, ILogger<SpatIntersectionRepository> logger) : base(context, logger)
    {
    }

    public async Task<ICollection<SpatIntersectionSync>> GetByTenantAsync(Guid id)
    {
        var filter = Builders<SpatIntersectionSync>.Filter.Where(x => x.TenantId == id);

        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(filter));
        return results.ToList();
    }
}