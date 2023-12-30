namespace Profile.Infra.Context.Uow;

public interface IUnitOfWork
{
    Task CommitAsync();
}