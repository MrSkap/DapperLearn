using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public interface IAnimalService
{
    Task<IEnumerable<Animal>> GetAllAnimalsAsync(IUnitOfWork uow);
    Task<Animal?> GetAnimalByIdAsync(Guid id, IUnitOfWork uow);
    Task CreateAnimalAsync(Animal animal, IUnitOfWork uow);
    Task UpdateAnimalAsync(Animal animal, IUnitOfWork uow);
    Task DeleteAnimalAsync(Guid animalId, IUnitOfWork uow);
    Task<AnimalLocation?> GetAnimalLocationAsync(Guid id, IUnitOfWork uow);
}