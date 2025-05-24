using System.Data;
using Dapper;
using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public class StatisticRepository : IStatisticRepository
{
    private readonly IDbConnection _connection;

    public StatisticRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<AgeStatistics?> GetAgeStatistics()
    {
        var queryMaxAndAvg = @"
            select * from animals where age = (select max(age) from animals);
            select avg(age) from animals;";

        var queryMin = @"select * from animals where age = (select min(age) from animals);";

        await using var multi = await _connection.QueryMultipleAsync(queryMaxAndAvg);

        var oldest = await multi.ReadFirstAsync<Animal>();
        var average = await multi.ReadFirstAsync<double>();
        var youngest = (await _connection.QueryAsync<Animal>(queryMin)).FirstOrDefault();
        return new AgeStatistics
        {
            OldestAnimal = oldest,
            YoungestAnimal = youngest,
            AverageAge = average
        };
    }

    public async Task<NamingStatistics?> GetNamingStatistics()
    {
        var queryAll = @"
            select name from animals 
            union all select name from aviaries;
        ";
        var queryUnique = @"
            select name from animals 
            union select name from aviaries;
        ";
        var queryDupl = @"
            select name from animals 
            intersect all select name from aviaries;
        ";

        var all = (await _connection.QueryAsync<string>(queryAll)).ToList();
        var unique = (await _connection.QueryAsync<string>(queryUnique)).ToList();
        var duplicates = (await _connection.QueryAsync<string>(queryDupl)).ToList();

        return new NamingStatistics
        {
            AllUsedNames = all,
            AllUniqueUsedNames = unique,
            DuplicateNames = duplicates
        };
    }
}