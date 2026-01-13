using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace AGRONET.Auth.Infrastructure.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _config;

        public SqlConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateBdAgronetConnection()
        {
            var cs = _config.GetConnectionString("BD_AGRONET")
                     ?? throw new InvalidOperationException("ConnectionString 'BD_AGRONET' no está configurado.");
            return new SqlConnection(cs);
        }
    }
}
