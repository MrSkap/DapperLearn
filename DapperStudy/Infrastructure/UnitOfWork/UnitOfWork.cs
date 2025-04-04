using System.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private static readonly ILogger Logger = Log.ForContext<UnitOfWork>();
    private readonly IDbConnection _connection;

    private readonly Dictionary<Type, Func<IDbConnection, object>> _repositoryFactory =
        new()
        {
            {
                typeof(IAnimalRepository), con => new AnimalRepository(con)
            }
        };

    private readonly IDbTransaction? _transaction;

    public UnitOfWork(IDbConnection connection, bool isTransactional = true)
    {
        _connection = connection;
        if (isTransactional)
            _transaction = connection.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
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

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while commit");
            throw;
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while rollback");
            throw;
        }
    }
}