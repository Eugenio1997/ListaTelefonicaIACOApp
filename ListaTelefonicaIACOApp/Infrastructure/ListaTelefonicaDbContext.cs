using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ListaTelefonicaIACOApp.Infrastructure
{
    public class ListaTelefonicaDbContext
    {

        private readonly string? _connectionString;

        public ListaTelefonicaDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ListaTelefonicaIACOConnectionString");
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var conn = CreateConnection();
            await conn.OpenAsync();
            return conn;
        }

    }
}
