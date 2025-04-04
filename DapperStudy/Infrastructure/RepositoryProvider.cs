using System.Data;
using Npgsql;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Infrastructure;

public class RepositoryProvider : IRepositoryProvider, IDisposable
{
    private static readonly ILogger Logger = Log.ForContext<RepositoryProvider>();
    private static IDbConnection? _connection;

    private readonly string _connectionString;

    private readonly Dictionary<Type, Func<IDbConnection, object>> _repositoryFactory =
        new()
        {
            {
                typeof(IAnimalRepository), con => { return new AnimalRepository(con); }
            }
        };

    public RepositoryProvider(string connectionString)
    {
        _connectionString = connectionString;
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();
        var transaction = _connection.BeginTransaction();
        transaction.Commit();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    public T GetRepository<T>() where T : class
    {
        try
        {
            if (_connection is null || _connection.State == ConnectionState.Closed)
                throw new NullReferenceException($"No connection for {typeof(T).Name}");
            var repository = _repositoryFactory[typeof(T)].Invoke(_connection) as T;
            if (repository is null)
                throw new NullReferenceException($"No repository found for type {typeof(T).Name}");
            return repository;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while getting repository");
            throw;
        }
    }
}