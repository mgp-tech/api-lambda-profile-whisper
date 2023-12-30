namespace Profile.Infra.Repository.Base;

public class DeleteRepositoryBase<T> : IDeleteRepositoryBase<T> where T : BaseEntity
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<DeleteRepositoryBase<T>> _logger;

    public DeleteRepositoryBase(DatabaseContext databaseContext, ILogger<DeleteRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task ExecuteAsync<TKey>(TKey id) where TKey : struct
    {
        _logger.LogInformation("Starting to remove entity type of {type}", typeof(T).Name);

        _logger.LogInformation("Looking for the entity with id: {id} in database", id);
        var entity = await _databaseContext.Set<T>().FindAsync(id);

        if (entity is null)
        {
            _logger.LogWarning("Record not exists in database with id: {id}", id);
            throw new NotFoundException($"Record not exists in database with id: {id}");
        }

        _logger.LogInformation("Deleting entity");
        _databaseContext.Remove(entity);
        _logger.LogInformation("Entity {type} delete finalized", typeof(T).Name);
    }
}