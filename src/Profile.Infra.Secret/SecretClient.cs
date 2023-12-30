namespace Profile.Infra.Secret;

public class SecretClient : ISecretClient
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly ILogger<SecretClient> _logger;

    public SecretClient(IAmazonSecretsManager secretsManager, ILogger<SecretClient> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }

    public async Task<object> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new SecretClientException("Secrets name can't be null or empty");

        var result = await _secretsManager.GetSecretValueAsync(new GetSecretValueRequest
        {
            SecretId = name
        });

        if (result is null) throw new NotFoundException("The secret couldn't be found");
        return result.SecretString;
    }
}