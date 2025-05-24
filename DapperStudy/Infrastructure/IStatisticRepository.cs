using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public interface IStatisticRepository
{
    Task<AgeStatistics?> GetAgeStatistics();
    Task<NamingStatistics?> GetNamingStatistics();
}