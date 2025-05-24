using System.Data;
using Dapper;
using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public class AnimalRepository : IAnimalRepository
{
    private readonly IDbConnection _connection;

    public AnimalRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
    {
        var query = @"select * from animals";
        return await _connection.QueryAsync<Animal>(query);
    }

    public Task<IEnumerable<Animal>> GetAnimalsByAviaryIdAsync(Guid aviaryId)
    {
        throw new NotImplementedException();
    }

    public async Task<Animal?> GetAnimalByIdAsync(Guid id)
    {
        var query = @"select * from animals where id = @id";
        return await _connection.QuerySingleOrDefaultAsync<Animal>(query, new { id });
    }

    public async Task CreateAnimalAsync(Animal animal)
    {
        var insert = $@"INSERT INTO animals
            VALUES
            (@{nameof(Animal.Id)},
            @{nameof(Animal.Name)},
            @{nameof(Animal.Age)},
            @{nameof(Animal.Weight)},
            @{nameof(Animal.AviaryId)})";
        await _connection.QueryAsync(insert, animal);
    }

    public async Task UpdateAnimalAsync(Animal animal)
    {
        var query = $@"UPDATE animals 
            SET 
                name = @{nameof(Animal.Name)},
                age = @{nameof(Animal.Age)},
                weight = @{nameof(Animal.Weight)}
                aviaryId = @{nameof(Animal.AviaryId)}
            WHERE id = @{nameof(Animal.Id)}";
        await _connection.QueryAsync(query, animal);
    }

    public async Task DeleteAnimalAsync(Guid animalId)
    {
        var query = @"DELETE from animals where id = @id";
        await _connection.ExecuteAsync(query, animalId);
    }

    public async Task<AnimalLocation?> GetAnimalLocation(Guid animalId)
    {
        var query = @"
            select * from animals where id = @id1;
            select name from aviaries where id = (select ""aviaryId"" from animals where id = @id2);
        ";

        await using var multi = await _connection.QueryMultipleAsync(query, new {id1 = animalId, id2 = animalId});
        var animal = await multi.ReadFirstAsync<Animal>();
        var location = await multi.ReadFirstAsync<string>();
        
        if(animal is not null && location is not null)
        {
            return new AnimalLocation
            {
                Animal = animal,
                AviaryName = location
            };
        }

        return null;
    }
}