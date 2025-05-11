using System.Data;
using Dapper;
using DapperStudy.Infrastructure.Entities;
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

    public async Task<Aviary?> GetAviaryAsync(Guid id)
    {
        var cmdTxt = @"SELECT aviary.*, setting.*, animal.* 
            FROM aviaries aviary
            LEFT OUTER JOIN aviary_settings setting ON aviary.SettingsId = setting.Id
            LEFT OUTER JOIN animals animal ON aviary.Id = animal.AviaryId
            WHERE aviary.Id = @id";

        var results =
            (await _connection
                .QueryAsync<AviaryEntity, AviarySettingsEntity, AnimalEntity,
                    Tuple<AviaryEntity, AviarySettingsEntity, AnimalEntity>>(cmdTxt,
                    (aviary, setting, animal) =>
                        Tuple.Create(aviary, (AviarySettingsEntity)setting, (AnimalEntity)animal),
                    new { id }))
            .ToList();

        // get aviary entity
        var aviaryEntity = results.First().Item1;
        if (aviaryEntity is null) return null;

        var aviary = aviaryEntity.MapToModel();
        if (results.Any())
        {
            // get aviary settings entity
            var settings = results.FirstOrDefault(n => n.Item2 != null).Item2;

            if (settings is not null) aviary.Settings = settings.MapToModel();

            // get all animals for aviary from animal entities
            var animals = results.Where(n => n.Item3 != null).Select(n => n.Item3).ToList();
            aviary.Animals = animals.Where(a => a.AviaryId == aviary.Id).Select(a => a.MapToModel()).ToList();
        }

        return aviary;
    }

    public async Task AddAviaryAsync(Aviary aviary)
    {
        var model = new AviaryEntity(aviary.Id, aviary.Name, aviary.Settings.Id);
        var insert = $@"INSERT INTO aviaries
            VALUES
            (@{nameof(AviaryEntity.Id)},
            @{nameof(AviaryEntity.Name)},
            @{nameof(AviaryEntity.SettingsId)})";

        await _connection.QueryAsync(insert, model);
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