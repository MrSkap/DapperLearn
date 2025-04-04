using System.Data;
using Dapper;
using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public class AviaryRepository : IAviaryRepository
{
    private readonly IDbConnection _connection;

    public AviaryRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public Task<IEnumerable<Aviary>> GetAviariesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Aviary> GetAviaryAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAviaryAsync(Aviary aviary)
    {
        var insert = $@"INSERT INTO aviaries
            (Id, Name, SettingsId) VALUES
            (@{nameof(Aviary.Id)},
            @{nameof(Aviary.Name)},
            ""SettingsId"")";

        await _connection.QueryAsync(insert, new
        {
            aviary.Id,
            aviary.Name,
            settingsId = aviary.Settings.Id
        });
    }

    public async Task AddAviarySettingsAsync(AviarySettings settings)
    {
        var insert = $@"INSERT INTO aviary_settings
            (Id, Type, Width, Height, Depth) VALUES
            (@{nameof(AviarySettings.Id)},
            @{nameof(AviarySettings.Type)},
            @{nameof(AviarySettings.Width)},
            @{nameof(AviarySettings.Height)},
             @{nameof(AviarySettings.Depth)})";
        await _connection.QueryAsync(insert, settings);
    }

    public Task UpdateAviaryAsync(Aviary aviary)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAviaryAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AviarySummary>> GetAviarySummaryAsync(Guid id)
    {
        var query = @"
            select 
                [AviarySummary]
            from aviaries
                left join aviary_settings 
                    on aviaries.SettingsId = aviary_settings.Id
                left join 
                    (select 
                         aviaryId,
                         count(*) as AnimalsCount
                     from animals group by AviaryId) as animals
                    on aviaries.Id = animals.aviaryId;";
        var aviaries = await _connection.QueryAsync<AviarySummary>(query);
        return aviaries.ToList();
    }
}