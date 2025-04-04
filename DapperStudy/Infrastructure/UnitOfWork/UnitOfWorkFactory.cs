using System.Data;
using Npgsql;

namespace DapperStudy.Infrastructure.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory, IDisposable
{
    private readonly IDbConnection _connection;

    public UnitOfWorkFactory(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public UnitOfWork Create()
    {
        return new UnitOfWork(_connection);
    }

    public UnitOfWork CreateNonTransactional()
    {
        return new UnitOfWork(_connection, false);
    }
}