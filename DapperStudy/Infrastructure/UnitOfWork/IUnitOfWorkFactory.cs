namespace DapperStudy.Infrastructure.UnitOfWork;

public interface IUnitOfWorkFactory
{
    UnitOfWork Create();
    UnitOfWork CreateNonTransactional();
}