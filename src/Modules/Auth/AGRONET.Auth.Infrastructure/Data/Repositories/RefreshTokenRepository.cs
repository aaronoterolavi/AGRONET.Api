using AGRONET.Auth.Application.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Data.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public RefreshTokenRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task InsertAsync(int idUsuario, byte[] tokenHash, DateTime expiresAt, string? ip, string? userAgent, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario, DbType.Int32);
            p.Add("@TokenHash", tokenHash, DbType.Binary, size: 32);
            p.Add("@ExpiresAt", expiresAt, DbType.DateTime);
            p.Add("@CreatedIp", ip, DbType.String);
            p.Add("@UserAgent", userAgent, DbType.String);

            await con.ExecuteAsync("dbo.USP_RefreshToken_Insertar", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<RefreshTokenRecord?> GetByHashAsync(byte[] tokenHash, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@TokenHash", tokenHash, DbType.Binary, size: 32);

            return await con.QuerySingleOrDefaultAsync<RefreshTokenRecord>(
                "dbo.USP_RefreshToken_ObtenerPorHash",
                p,
                commandType: CommandType.StoredProcedure);
        }

        public async Task RevokeAsync(byte[] tokenHash, string? reason, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@TokenHash", tokenHash, DbType.Binary, size: 32);
            p.Add("@ReasonRevoked", reason, DbType.String);

            await con.ExecuteAsync("dbo.USP_RefreshToken_Revocar", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<(int Codigo, string Mensaje)> RotateAsync(byte[] oldHash, byte[] newHash, DateTime newExpiresAt, string? ip, string? userAgent, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@OldTokenHash", oldHash, DbType.Binary, size: 32);
            p.Add("@NewTokenHash", newHash, DbType.Binary, size: 32);
            p.Add("@NewExpiresAt", newExpiresAt, DbType.DateTime);
            p.Add("@CreatedIp", ip, DbType.String);
            p.Add("@UserAgent", userAgent, DbType.String);

            p.Add("@Codigo", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            await con.ExecuteAsync("dbo.USP_RefreshToken_Rotar", p, commandType: CommandType.StoredProcedure);

            return (p.Get<int>("@Codigo"), p.Get<string>("@Mensaje"));
        }

        public async Task RevokeByUserAsync(int idUsuario, byte[] tokenHash, string? reason, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario, DbType.Int32);
            p.Add("@TokenHash", tokenHash, DbType.Binary, size: 32);
            p.Add("@ReasonRevoked", reason, DbType.String);

            await con.ExecuteAsync("dbo.USP_RefreshToken_RevocarPorUsuario", p, commandType: CommandType.StoredProcedure);
        }

    }
}
