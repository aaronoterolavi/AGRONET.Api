using AGRONET.Roles.Application.Contracts;
using AGRONET.Roles.Application.Interfaces;
using AGRONET.Roles.Infrastructure.Data;
using Dapper;
using System.Data;

namespace AGRONET.Roles.Infrastructure.Repositories
{
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly ISqlConnectionFactory _cn;

        public RoleRepository(ISqlConnectionFactory cn)
        {
            _cn = cn;
        }

        public async Task<List<RoleDto>> ListarAsync(CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            var rows = await conn.QueryAsync<RoleDto>(
                sql: "dbo.USP_Roles_Listar",
                commandType: CommandType.StoredProcedure
            );

            return rows.ToList();
        }

        public async Task<RoleDto?> ObtenerPorIdAsync(int idRol, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<RoleDto>(
                sql: "dbo.USP_Roles_ObtenerPorId",
                param: new { IdRol = idRol },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> CrearAsync(CreateRoleRequestDto req, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            // Devuelve IdRol
            return await conn.QuerySingleAsync<int>(
                sql: "dbo.USP_Roles_Crear",
                param: new
                {
                    Codigo = req.Codigo,
                    Nombre = req.Nombre,
                    Descripcion = req.Descripcion
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task ActualizarAsync(int idRol, UpdateRoleRequestDto req, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            await conn.ExecuteAsync(
                sql: "dbo.USP_Roles_Actualizar",
                param: new
                {
                    IdRol = idRol,
                    Nombre = req.Nombre,
                    Descripcion = req.Descripcion
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task CambiarActivoAsync(int idRol, bool activo, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            await conn.ExecuteAsync(
                sql: "dbo.USP_Roles_CambiarActivo",
                param: new
                {
                    IdRol = idRol,
                    Activo = activo
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
