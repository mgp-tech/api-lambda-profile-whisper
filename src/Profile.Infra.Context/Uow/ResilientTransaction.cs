namespace Profile.Infra.Context.Uow;

public sealed class ResilientTransaction
{
    private readonly DatabaseContext _databaseContext;

    private ResilientTransaction(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    private IDbContextTransaction? Transaction { get; set; }

    public static ResilientTransaction New(DatabaseContext context)
    {
        return new ResilientTransaction(context);
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        var strategy = _databaseContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            try
            {
                Transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                await action();
                await Transaction.CommitAsync();
            }
            catch
            {
                if (Transaction != null)
                    await Transaction.RollbackAsync();
            }
            finally
            {
                if (Transaction != null)
                    await Transaction.DisposeAsync();

                await _databaseContext.DisposeAsync();
            }
        });
    }
}