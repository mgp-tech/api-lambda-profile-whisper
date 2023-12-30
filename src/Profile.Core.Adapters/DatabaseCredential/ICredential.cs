namespace Profile.Core.Adapters.DatabaseCredential;

public interface ICredential
{
    Task<string> ExecuteAsync();
}