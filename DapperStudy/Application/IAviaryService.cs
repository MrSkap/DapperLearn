using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Models;

namespace DapperStudy.Application;

public interface IAviaryService
{
    public Task<IEnumerable<Aviary>> GetAviariesAsync(IUnitOfWork unitOfWork);
    public Task<Aviary> GetAviaryAsync(Guid id, IUnitOfWork unitOfWork);
    public Task AddAviaryAsync(Aviary aviary, IUnitOfWork unitOfWork);
    public Task UpdateAviaryAsync(Aviary aviary, IUnitOfWork unitOfWork);
    public Task DeleteAviaryAsync(Guid id, IUnitOfWork unitOfWork);
    public Task<List<AviarySummary>> GetAviarySummariesAsync(IUnitOfWork unitOfWork);
}