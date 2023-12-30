namespace Profile.Core.Adapters.Repository.Base;

public interface IQueryRepositoryBase<out T> where T : class
{
    IQueryable<T> Query { get; }
}