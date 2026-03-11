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
    }
}