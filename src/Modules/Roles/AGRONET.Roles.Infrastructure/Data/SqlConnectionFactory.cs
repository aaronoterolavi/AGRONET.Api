using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AGRONET.Roles.Infrastructure.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("No existe ConnectionStrings:DefaultConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_cs);
    }
}
