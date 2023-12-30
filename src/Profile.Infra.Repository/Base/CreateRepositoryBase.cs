namespace Profile.Infra.Repository.Base;

public class CreateRepositoryBase<T> : ICreateRepositoryBase<T> where T : BaseEntity
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<CreateRepositoryBase<T>> _logger;

    public CreateRepositoryBase(DatabaseContext databaseContext, ILogger<CreateRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(T entity)
    {
        _logger.LogInformation("Initialize to create a new entity type {type}", typeof(T).Name);

        if (entity is null)
            throw new RepositoryException($"{typeof(T).Name} can't be null");

        if (!Guid.Empty.Equals(entity.Id))
            throw new RepositoryException($"{typeof(T).Name} property {nameof(entity.Id)} must be empty");

        var type = entity.GetType();
        var props = type.GetProperty(nameof(entity.Id),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        props?.SetValue(entity, Guid.NewGuid(), null);

        await _databaseContext.AddAsync(entity);

        _logger.LogInformation("Finish to create a new entity type {type}", typeof(T).Name);
    }
}