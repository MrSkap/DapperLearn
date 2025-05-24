using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public interface IStatisticService
{
    Task<AgeStatistics?> GetAgeStatistics(IUnitOfWork uow);
    Task<NamingStatistics?> GetNamingStatistics(IUnitOfWork uow);
}