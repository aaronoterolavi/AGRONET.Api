using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace AGRONET.Menus.Infrastructure.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(IConfiguration config)
        {
            // Debe existir en appsettings.json: ConnectionStrings:DefaultConnection
            _cs = config.GetConnectionString("BD_AGRONET")
                  ?? throw new InvalidOperationException("No existe ConnectionStrings:BD_AGRONET");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_cs);
    }
}
