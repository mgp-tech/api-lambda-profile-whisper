namespace Profile.Core.Adapters.Repository.Base;

public interface IGetRepositoryBase<T> where T : class
{
    Task<IEnumerable<T>?> ExecuteAsync();
}