using Econolite.Ode.Models.SpatCorridorSync;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Repository.SpatCorridorSync;

public class SpatCorridorRepository: GuidDocumentRepositoryBase<CorridorSync>, ISpatCorridorRepository
{
    public SpatCorridorRepository(IMongoContext context, ILogger<SpatCorridorRepository> logger) : base(context, logger)
    {
    }

    public async Task<ICollection<CorridorSync>> GetByTenantAsync(Guid id)
    {
        var filter = Builders<CorridorSync>.Filter.Where(x => x.TenantId == id);

        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(filter));
        return results.ToList();
    }
}