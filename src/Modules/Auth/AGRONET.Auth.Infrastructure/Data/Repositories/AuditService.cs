using AGRONET.Auth.Application.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Data.Repositories
{
    public sealed class AuditService : IAuditService
    {
        private readonly ISqlConnectionFactory _factory;

        public AuditService(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task LogAsync(string? username, string? dniNorm, string resultado, string? mensaje, string? ip, string? userAgent, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@Username", username, DbType.String);
            p.Add("@DniNorm", dniNorm, DbType.String);
            p.Add("@Resultado", resultado, DbType.String);
            p.Add("@Mensaje", mensaje, DbType.String);
            p.Add("@Ip", ip, DbType.String);
            p.Add("@UserAgent", userAgent, DbType.String);

            await con.ExecuteAsync("dbo.USP_AuditLogin_Insertar", p, commandType: CommandType.StoredProcedure);
        }
    }
}
