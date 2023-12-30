using Microsoft.EntityFrameworkCore;

namespace Profile.Infra.Repository.Base;

public class GetRepositoryBase<T> : IGetRepositoryBase<T> where T : class
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<GetRepositoryBase<T>> _logger;

    public GetRepositoryBase(DatabaseContext databaseContext, ILogger<GetRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<IEnumerable<T>?> ExecuteAsync()
    {
        _logger.LogInformation("Starting to get type of {type}", typeof(T).Name);
        var result = await _databaseContext.Set<T>().ToListAsync();

        _logger.LogInformation("Obtained entities: {type}", typeof(T).Name);
        return result;
    }
}