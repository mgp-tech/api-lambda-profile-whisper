namespace Profile.Test.Unit.Infra.Mocks;

[ExcludeFromCodeCoverage]
public class DatabaseContextStub : DatabaseContext
{
    public DatabaseContextStub(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger,
        ICredential credential) : base(
        options, logger, credential)
    {
    }

    public DbSet<EntityTest>? EntityTests { get; set; }
}