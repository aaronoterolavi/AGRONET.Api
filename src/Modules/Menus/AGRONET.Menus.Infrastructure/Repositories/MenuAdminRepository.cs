using AGRONET.Menus.Application.Contracts;
using AGRONET.Menus.Application.Interfaces;
using AGRONET.Menus.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.Menus.Infrastructure.Repositories
{
    public sealed class MenuAdminRepository : IMenuAdminRepository
    {
        private readonly ISqlConnectionFactory _cn;

        public MenuAdminRepository(ISqlConnectionFactory cn)
        {
            _cn = cn;
        }

        public async Task<List<MenuAdminDto>> ListarTodosAsync(bool soloActivos, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            var rows = await conn.QueryAsync<MenuAdminDto>(
                sql: "dbo.USP_Menu_ListarTodos",
                param: new { SoloActivos = soloActivos },
                commandType: CommandType.StoredProcedure
            );

            return rows.ToList();
        }

        public async Task<MenuAdminDto?> ObtenerPorIdAsync(int idMenu, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<MenuAdminDto>(
                sql: "dbo.USP_Menu_ObtenerPorId",
                param: new { IdMenu = idMenu },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> CrearAsync(CreateMenuRequestDto req, string usuario, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            return await conn.QuerySingleAsync<int>(
                sql: "dbo.USP_Menu_Crear",
                param: new
                {
                    IdMenuPadre = req.IdMenuPadre,
                    Nombre = req.Nombre,
                    Descripcion = req.Descripcion,
                    Ruta = req.Ruta,
                    Orden = req.Orden ?? 0,
                    Usuario = usuario
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task ActualizarAsync(int idMenu, UpdateMenuRequestDto req, string usuario, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            await conn.ExecuteAsync(
                sql: "dbo.USP_Menu_Actualizar",
                param: new
                {
                    IdMenu = idMenu,
                    IdMenuPadre = req.IdMenuPadre,
                    Nombre = req.Nombre,
                    Descripcion = req.Descripcion,
                    Ruta = req.Ruta,
                    Orden = req.Orden ?? 0,
                    Usuario = usuario
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task CambiarActivoAsync(int idMenu, bool activo, string usuario, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            await conn.ExecuteAsync(
                sql: "dbo.USP_Menu_CambiarActivo",
                param: new
                {
                    IdMenu = idMenu,
                    Activo = activo,
                    Usuario = usuario
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task CambiarOrdenAsync(int idMenu, int orden, string usuario, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            await conn.ExecuteAsync(
                sql: "dbo.USP_Menu_CambiarOrden",
                param: new
                {
                    IdMenu = idMenu,
                    Orden = orden,
                    Usuario = usuario
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
