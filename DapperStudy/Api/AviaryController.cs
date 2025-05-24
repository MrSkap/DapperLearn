using DapperStudy.Application;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperStudy.Api;

[ApiController]
[Route("api/aviary")]
public class AviaryController : ControllerBase
{
    private readonly IAviaryService _aviaryService;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AviaryController(IUnitOfWorkFactory unitOfWorkFactory, IAviaryService aviaryService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _aviaryService = aviaryService;
    }

    [HttpGet("all")]
    public async Task<IEnumerable<Aviary>> GetAllAviariesAsync()
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _aviaryService.GetAviariesAsync(uow);
    }

    [HttpGet]
    public async Task<Aviary> GetAviaryAsync(Guid id)
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _aviaryService.GetAviaryAsync(id, uow);
    }

    [HttpDelete]
    public async Task DeleteAviaryAsync(Guid id)
    {
        using var uow = _unitOfWorkFactory.Create();
        try
        {
            await _aviaryService.DeleteAviaryAsync(id, uow);
            uow.Commit();
        }
        catch (Exception e)
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpPatch]
    public async Task UpdateAviaryAsync(Aviary aviary)
    {
        using var uow = _unitOfWorkFactory.Create();
        try
        {
            await _aviaryService.UpdateAviaryAsync(aviary, uow);
            uow.Commit();
        }
        catch (Exception e)
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpPut]
    public async Task CreateAviaryAsync(Aviary aviary)
    {
        using var uow = _unitOfWorkFactory.Create();
        try
        {
            await _aviaryService.AddAviaryAsync(aviary, uow);
            uow.Commit();
        }
        catch (Exception e)
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpGet("summaries")]
    public async Task<List<AviarySummary>> GetAviarySummaries()
    {
        using var uow = _unitOfWorkFactory.Create();
        return await _aviaryService.GetAviarySummariesAsync(uow);
    }
}