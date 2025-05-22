using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ListaTelefonicaIACOApp.Infrastructure
{
    public class ListaTelefonicaDbContext
    {

        private readonly string? _connectionString;

        public ListaTelefonicaDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ListaTelefonicaIACOLocalConnectionString");
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public IDbConnection GetOpenConnection()
        {
            var conn = CreateConnection();
            conn.Open();
            return conn;
        }

    }
}
