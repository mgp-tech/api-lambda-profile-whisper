namespace Profile.Core.Adapters.Secret;

public interface ISecretClient
{
    Task<object> GetAsync(string name);
}