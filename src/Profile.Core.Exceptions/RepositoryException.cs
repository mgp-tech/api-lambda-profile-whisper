namespace Profile.Core.Exceptions;

public class RepositoryException : Exception
{
    public RepositoryException(string message) : base(message)
    {
    }
}