using AGRONET.Menus.Application.Contracts;
using AGRONET.Menus.Application.Interfaces;
using AGRONET.Menus.Infrastructure.Data;
using Dapper;
using System.Data;

namespace AGRONET.Menus.Infrastructure.Repositories
{
    public sealed class MenuRepository : IMenuRepository
    {
        private readonly ISqlConnectionFactory _cn;

        public MenuRepository(ISqlConnectionFactory cn)
        {
            _cn = cn;
        }

        public async Task<List<MenuDto>> ListarPorRolAsync(int idRol, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            // Dapper no usa ct directo en QueryAsync clásico, pero igual está bien
            var rows = await conn.QueryAsync<MenuDto>(
                sql: "dbo.USP_Menus_ListarPorRol",
                param: new { IdRol = idRol },
                commandType: CommandType.StoredProcedure
            );

            return rows.ToList();
        }

        public async Task ReemplazarMenusDeRolAsync(int idRol, List<int> menuIds, string usuario, CancellationToken ct)
        {
            using var conn = _cn.CreateConnection();

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            foreach (var id in (menuIds ?? new List<int>()).Distinct())
                table.Rows.Add(id);

            var p = new DynamicParameters();
            p.Add("@IdRol", idRol, DbType.Int32);
            p.Add("@Usuario", usuario, DbType.String);
            p.Add("@MenuIds", table.AsTableValuedParameter("dbo.IntList"));

            await conn.ExecuteAsync(
                sql: "dbo.USP_RolMenu_Reemplazar",
                param: p,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
