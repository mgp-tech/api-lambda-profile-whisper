namespace Profile.Core.Adapters.Repository.Base;

public interface IDeleteRepositoryBase<in T> where T : BaseEntity
{
    Task ExecuteAsync<TKey>(TKey id) where TKey : struct;
}