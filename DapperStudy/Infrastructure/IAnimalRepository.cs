using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public interface IAnimalRepository
{
    Task<IEnumerable<Animal>> GetAllAnimalsAsync();
    Task<IEnumerable<Animal>> GetAnimalsByAviaryIdAsync(Guid aviaryId);
    Task<Animal?> GetAnimalByIdAsync(Guid id);
    Task CreateAnimalAsync(Animal animal);
    Task UpdateAnimalAsync(Animal animal);
    Task DeleteAnimalAsync(Guid animalId);
    Task<AnimalLocation?> GetAnimalLocation(Guid animalId);
}