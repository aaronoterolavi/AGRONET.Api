using AGRONET.Auth.Application.Contracts.Auth;
using AGRONET.Auth.Application.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Data.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public UsersRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();
            var p = new DynamicParameters();
            p.Add("@Username", username, DbType.String);

            var user = await con.QuerySingleOrDefaultAsync<UserDto>(
                sql: "dbo.USP_Usuarios_ObtenerPorUsername",
                param: p,
                commandType: CommandType.StoredProcedure);

            return user;
        }

        public async Task<UserDto?> GetByIdAsync(int idUsuario, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();
            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario, DbType.Int32);

            var user = await con.QuerySingleOrDefaultAsync<UserDto>(
                sql: "dbo.USP_Usuarios_ObtenerPorId",
                param: p,
                commandType: CommandType.StoredProcedure);

            return user;
        }

        public async Task<(int Codigo, string Mensaje, int NuevoId)> InsertFromRgpmaByDniAsync(
            string username, string dni, string? ip, string? userAgent, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();
            var p = new DynamicParameters();

            p.Add("@Username", username, DbType.String);
            p.Add("@Dni", dni, DbType.String);
            p.Add("@Ip", ip, DbType.String);
            p.Add("@UserAgent", userAgent, DbType.String);

            p.Add("@NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            p.Add("@Codigo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await con.ExecuteAsync(
                sql: "dbo.USP_Usuarios_InsertarDesdeRgpmaPorDni",
                param: p,
                commandType: CommandType.StoredProcedure);

            var codigo = p.Get<int>("@Codigo");
            var mensaje = p.Get<string>("@Mensaje");
            var nuevoId = p.Get<int>("@NuevoId");

            return (codigo, mensaje, nuevoId);
        }

        public async Task UpdateLastLoginAsync(int idUsuario, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();
            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario, DbType.Int32);

            await con.ExecuteAsync(
                sql: "dbo.USP_Usuarios_ActualizarUltimoLogin",
                param: p,
                commandType: CommandType.StoredProcedure);
        }
    }
}
