namespace Profile.Infra.Repository.Base;

public class QueryRepositoryBase<T> : IQueryRepositoryBase<T> where T : class
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<QueryRepositoryBase<T>> _logger;

    public QueryRepositoryBase(DatabaseContext databaseContext, ILogger<QueryRepositoryBase<T>> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public IQueryable<T> Query => _databaseContext.Set<T>().AsQueryable();
}