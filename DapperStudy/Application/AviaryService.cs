using DapperStudy.Infrastructure;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public class AviaryService : IAviaryService
{
    public async Task<IEnumerable<Aviary>> GetAviariesAsync(IUnitOfWork unitOfWork)
    {
        return await unitOfWork.GetRepository<IAviaryRepository>().GetAviariesAsync();
    }

    public async Task<Aviary> GetAviaryAsync(Guid id, IUnitOfWork unitOfWork)
    {
        return await unitOfWork.GetRepository<IAviaryRepository>().GetAviaryAsync(id);
    }

    public async Task AddAviaryAsync(Aviary aviary, IUnitOfWork unitOfWork)
    {
        await unitOfWork.GetRepository<IAviaryRepository>().AddAviaryAsync(aviary);
    }

    public async Task UpdateAviaryAsync(Aviary aviary, IUnitOfWork unitOfWork)
    {
        await unitOfWork.GetRepository<IAviaryRepository>().UpdateAviaryAsync(aviary);
    }

    public async Task DeleteAviaryAsync(Guid id, IUnitOfWork unitOfWork)
    {
        await unitOfWork.GetRepository<IAviaryRepository>().DeleteAviaryAsync(id);
    }

    public async Task<List<AviarySummary>> GetAviarySummariesAsync(Guid id, IUnitOfWork unitOfWork)
    {
        return await unitOfWork.GetRepository<IAviaryRepository>().GetAviarySummaryAsync(id);
    }
}