using AGRONET.Menus.Infrastructure.Data;
using AGRONET.Users.Application.Contracts;
using AGRONET.Users.Application.Interfaces;
using Dapper;
using System.Data;

namespace AGRONET.Users.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UserRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<UserDto>> ListarAsync(bool soloActivos, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<UserDto>(
                sql: "dbo.USP_Usuarios_Listar",
                param: new
                {
                    SoloActivos = soloActivos
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<UserDto?> ObtenerPorIdAsync(int idUsuario, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserDto>(
                sql: "dbo.USP_Usuarios_ObtenerPorId",
                param: new
                {
                    IdUsuario = idUsuario
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CrearAsync(CreateUserRequestDto request, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            var id = await connection.ExecuteScalarAsync<int>(
                sql: "dbo.USP_Usuarios_Crear",
                param: new
                {
                    request.Username,
                    request.Correo,
                    request.DniNorm,
                    request.ApePaterno,
                    request.ApeMaterno,
                    request.Nombres,
                    request.IdRol,
                    request.CodArea,
                    request.CodTipoEmpleado,
                    request.Flg_sede,
                    request.Activo,
                    request.descArea,
                    request.CodTrabajador
                },
                commandType: CommandType.StoredProcedure);

            return id;
        }

        public async Task ActualizarAsync(int idUsuario, UpdateUserRequestDto request, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                sql: "dbo.USP_Usuarios_Actualizar",
                param: new
                {
                    IdUsuario = idUsuario,
                    request.Username,
                    request.Correo,
                    request.DniNorm,
                    request.ApePaterno,
                    request.ApeMaterno,
                    request.Nombres,
                    request.IdRol,
                    request.CodArea,
                    request.CodTipoEmpleado,
                    request.Flg_sede,
                    request.Activo,
                    request.descArea,
                    request.CodTrabajador
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task CambiarActivoAsync(int idUsuario, bool activo, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                sql: "dbo.USP_Usuarios_CambiarActivo",
                param: new
                {
                    IdUsuario = idUsuario,
                    Activo = activo
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}