namespace Profile.Infra.Repository.Base;

public class UpdateRepositoryBase<T> : IUpdateRepositoryBase<T> where T : BaseEntity
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<UpdateRepositoryBase<T>> _logger;

    public UpdateRepositoryBase(DatabaseContext databaseContext, ILogger<UpdateRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(T entity)
    {
        _logger.LogInformation("Starting to update the entity type of {type}", typeof(T).Name);

        if (entity is null)
            throw new RepositoryException($"{typeof(T).Name} can't be null");

        if (Guid.Empty.Equals(entity.Id))
            throw new RepositoryException($"{typeof(T).Name} property {nameof(entity.Id)} can't be empty");

        _logger.LogInformation("Checking if the entity entity with id: {id} exists in track local", entity.Id);
        var local = _databaseContext.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);

        if (local is not null)
        {
            _logger.LogInformation("Detached local entity");
            _databaseContext.Entry(local).State = EntityState.Detached;
        }

        _logger.LogInformation("Looking for the entity with id: {id} in database", entity.Id);
        var current = await _databaseContext.Set<T>()
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(entity.Id));

        if (current is null)
        {
            _logger.LogWarning("No entity record found with id: {id}", entity.Id);
            throw new NotFoundException($"No entity record found with id: {entity.Id}");
        }

        _logger.LogInformation("Updating the entity");
        _databaseContext.Entry(current).CurrentValues.SetValues(entity);
        _databaseContext.Entry(current).State = EntityState.Modified;

        _logger.LogInformation("Entity {type} update finalized", typeof(T).Name);
    }
}