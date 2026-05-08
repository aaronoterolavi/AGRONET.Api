using AGRONET.Catalogos.Application.Contracts;
using AGRONET.Catalogos.Application.Interfaces;
using AGRONET.Menus.Infrastructure.Data;
using Dapper;
using System.Data;

namespace AGRONET.Catalogos.Infrastructure.Repositories
{
    public sealed class CatalogosRepository : ICatalogosRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public CatalogosRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<AreaComboDto>> ListarAreasAsync(CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<AreaComboDto>(
                sql: "dbo.USP_Areas_Combo",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        // ✅ NUEVO: Listar áreas padre (USP_AreasPadre)
        public async Task<IReadOnlyList<AreaComboDto>> ListarAreasPadreAsync(CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<AreaComboDto>(
                sql: "dbo.USP_AreasPadre",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        // ✅ NUEVO: Listar áreas hijas (USP_AreasHijas)
        public async Task<IReadOnlyList<AreaComboDto>> ListarAreasHijasAsync(string codPadre, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@cod_padre", codPadre);

            var result = await connection.QueryAsync<AreaComboDto>(
                sql: "dbo.USP_AreasHijas",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

    }
}