using DapperStudy.Infrastructure;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public class AnimalService : IAnimalService
{
    public async Task<IEnumerable<Animal>> GetAllAnimalsAsync(IUnitOfWork uow)
    {
        return await uow.GetRepository<IAnimalRepository>().GetAllAnimalsAsync();
    }

    public async Task<Animal?> GetAnimalByIdAsync(Guid id, IUnitOfWork uow)
    {
        return await uow.GetRepository<IAnimalRepository>().GetAnimalByIdAsync(id);
    }

    public Task CreateAnimalAsync(Animal animal, IUnitOfWork uow)
    {
        return uow.GetRepository<IAnimalRepository>().CreateAnimalAsync(animal);
    }

    public async Task UpdateAnimalAsync(Animal animal, IUnitOfWork uow)
    {
        await uow.GetRepository<IAnimalRepository>().UpdateAnimalAsync(animal);
    }

    public async Task DeleteAnimalAsync(Guid animalId, IUnitOfWork uow)
    {
        await uow.GetRepository<IAnimalRepository>().DeleteAnimalAsync(animalId);
    }
}