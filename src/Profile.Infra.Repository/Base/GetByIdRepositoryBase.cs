namespace Profile.Infra.Repository.Base;

public class GetByIdRepositoryBase<T> : IGetByIdRepositoryBase<T> where T : class
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<GetByIdRepositoryBase<T>> _logger;

    public GetByIdRepositoryBase(DatabaseContext databaseContext, ILogger<GetByIdRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<T?> ExecuteAsync<TKey>(TKey id) where TKey : struct
    {
        _logger.LogInformation("Starting to get type of {type}", typeof(T).Name);
        var result = await _databaseContext.Set<T>().FindAsync(id);

        _logger.LogInformation("Obtained entity: {type}", typeof(T).Name);
        return result;
    }
}