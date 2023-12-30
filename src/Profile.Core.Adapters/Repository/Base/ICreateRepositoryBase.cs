namespace Profile.Core.Adapters.Repository.Base;

public interface ICreateRepositoryBase<in T> where T : BaseEntity
{
    Task ExecuteAsync(T entity);
}