namespace DapperStudy.Infrastructure.UnitOfWork;

public interface IUnitOfWork
{
    T GetRepository<T>() where T : class;
    void Commit();
    void Rollback();
}