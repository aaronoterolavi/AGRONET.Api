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
            _cs = config.GetConnectionString("BD_AGRONET")
                  ?? throw new InvalidOperationException("No existe ConnectionStrings:BD_AGRONET");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_cs);
    }
}
