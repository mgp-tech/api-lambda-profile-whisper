namespace Profile.Infra.DatabaseCredential;

public class Credential : ICredential
{
    private readonly ISecretClient _secretClient;
    private readonly ILogger<Credential> _logger;

    public Credential(ISecretClient secretClient, ILogger<Credential> logger)
    {
        _secretClient = secretClient;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync()
    {
        _logger.LogInformation("Initialize to obtain database secrets");
        var variable = Environment.GetEnvironmentVariable("database-secret");

        if (string.IsNullOrEmpty(variable))
            throw new NotFoundException("Environment variable not found: database-secret");

        var secrets = await _secretClient.GetAsync(variable);
        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(secrets as string ?? string.Empty);

        if (result is null)
            throw new InvalidCastException(
                "It wasn't possible to convert the secrets into a dictionary! The secrets were null or empty!");

        if (!result.TryGetValue("host", out var host) || !result.TryGetValue("password", out var password) ||
            !result.TryGetValue("database", out var database) || !result.TryGetValue("user", out var user))
            throw new NotFoundException("Secret key not exists!");


        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database) ||
            string.IsNullOrEmpty(user))
            throw new NotFoundException("Not found secret value!");

        _logger.LogInformation("Complete to obtain the database secrets");
        return $"Server={host};Database={database};Uid={user};Pwd={password};CharSet=utf8";
    }
}