namespace Profile.Core.Adapters.Repository.Base;

public interface IGetByIdRepositoryBase<T> where T : class
{
    Task<T?> ExecuteAsync<TKey>(TKey id) where TKey : struct;
}