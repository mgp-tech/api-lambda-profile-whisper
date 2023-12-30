namespace Profile.Infra.Context.Uow;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(DatabaseContext databaseContext, ILogger<UnitOfWork> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task CommitAsync()
    {
        await ResilientTransaction.New(_databaseContext)
            .ExecuteAsync(async () =>
            {
                try
                {
                    await _databaseContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError("Occurred an error when try to commit transaction: {message}", e.Message);
                    throw;
                }
            });
    }
}