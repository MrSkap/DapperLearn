namespace DapperStudy.Infrastructure;

public interface IRepositoryProvider
{
    T GetRepository<T>() where T : class;
}