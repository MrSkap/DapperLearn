using DapperStudy.Infrastructure;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public class StatisticService : IStatisticService
{
    public async Task<AgeStatistics?> GetAgeStatistics(IUnitOfWork uow)
    {
        return await uow.GetRepository<IStatisticRepository>().GetAgeStatistics();
    }

    public async Task<NamingStatistics?> GetNamingStatistics(IUnitOfWork uow)
    {
        return await uow.GetRepository<IStatisticRepository>().GetNamingStatistics();
    }
}