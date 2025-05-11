using DapperStudy.Models;

namespace DapperStudy.Infrastructure;

public interface IAviaryRepository
{
    Task<IEnumerable<Aviary>> GetAviariesAsync();
    Task<Aviary?> GetAviaryAsync(Guid id);
    Task AddAviaryAsync(Aviary aviary);
    Task AddAviarySettingsAsync(AviarySettings settings);
    Task UpdateAviaryAsync(Aviary aviary);
    Task DeleteAviaryAsync(Guid id);
    Task<List<AviarySummary>> GetAviarySummaryAsync(Guid id);
}