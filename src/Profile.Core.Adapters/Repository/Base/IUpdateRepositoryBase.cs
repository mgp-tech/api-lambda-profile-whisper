namespace Profile.Core.Adapters.Repository.Base;

public interface IUpdateRepositoryBase<in T> where T : BaseEntity
{
    Task ExecuteAsync(T entity);
}