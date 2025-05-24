using DapperStudy.Application;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperStudy.Api;

[ApiController]
[Route("api")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AnimalController(IAnimalService animalService, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _animalService = animalService;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    [HttpGet("animals")]
    public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _animalService.GetAllAnimalsAsync(uow);
    }

    [HttpGet("animal/{animalId}")]
    public async Task<Animal?> GetAnimalAsync(Guid animalId)
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _animalService.GetAnimalByIdAsync(animalId, uow);
    }

    [HttpGet("animal/{animalId}/location")]
    public async Task<AnimalLocation?> GetAnimalLocationAsync(Guid animalId)
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _animalService.GetAnimalLocationAsync(animalId, uow);
    }

    [HttpDelete("animal/{animalId}")]
    public async Task RemoveAnimalById(Guid animalId)
    {
        using var uow = _unitOfWorkFactory.Create();
        await _animalService.DeleteAnimalAsync(animalId, uow);
        uow.Commit();
    }

    [HttpPut("create/animal")]
    public async Task CreateAnimal(CreateAnimalRequest createAnimalRequest)
    {
        using var uow = _unitOfWorkFactory.Create();
        await _animalService.CreateAnimalAsync(new Animal
        {
            Id = Guid.NewGuid(),
            Name = createAnimalRequest.Name,
            Age = createAnimalRequest.Age,
            Weight = createAnimalRequest.Weight
        }, uow);
    }

    [HttpPatch("animal/{animalId}")]
    public async Task UpdateAnimalById(Guid animalId, Animal animal)
    {
        using var uow = _unitOfWorkFactory.Create();
        await _animalService.UpdateAnimalAsync(animal, uow);
        uow.Commit();
    }
}