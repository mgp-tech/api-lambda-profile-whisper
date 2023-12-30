namespace Profile.Core.Exceptions;

public class SecretClientException : Exception
{
    public SecretClientException(string message) : base(message)
    {
    }
}