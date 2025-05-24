namespace DapperStudy.Models;

public class NamingStatistics
{
    public List<string> AllUsedNames { get; set; } = [];
    public List<string> DuplicateNames { get; set; } = [];
    public List<string> AllUniqueUsedNames { get; set; } = [];
}