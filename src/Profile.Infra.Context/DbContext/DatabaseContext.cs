namespace Profile.Infra.Context.DbContext;

public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly ILogger<DatabaseContext> _logger;
    private readonly ICredential _credential;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger,
        ICredential credential) : base(options)
    {
        _logger = logger;
        _credential = credential;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _logger.LogInformation("Start configuring the database context connection");

         var connectionString = _credential.ExecuteAsync().Result;
        
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        _logger.LogInformation("Finish configuring the database context connection");
        base.OnConfiguring(optionsBuilder);
    }
}