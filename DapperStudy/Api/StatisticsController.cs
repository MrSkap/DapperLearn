using DapperStudy.Api.Filters;
using DapperStudy.Application;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperStudy.Api;

[ApiController]
[Route("api/stat")]
[ServiceFilter(typeof(BadResponseFilter))]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticService _statisticService;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public StatisticsController(IStatisticService statisticService, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _statisticService = statisticService;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    [HttpGet("age")]
    public async Task<AgeStatistics?> GetAgeStatistics()
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _statisticService.GetAgeStatistics(uow);
    }

    [HttpGet("name")]
    public async Task<NamingStatistics?> GetNamingStatistics()
    {
        using var uow = _unitOfWorkFactory.CreateNonTransactional();
        return await _statisticService.GetNamingStatistics(uow);
    }
}