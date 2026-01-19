using AGRONET.Auth.Infrastructure.Data; // reutilizas tu ISqlConnectionFactory de Auth (o muévelo a Common)
using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using AGRONET.FichaSalida.Application.Interfaces;
using Dapper;
using System.Data;

namespace AGRONET.FichaSalida.Infrastructure.Data.Repositories
{
    public sealed class FichaSalidaRepository : IFichaSalidaRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public FichaSalidaRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyList<FichaSalidaTipoDto>> ListarTiposAsync(CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var rows = await con.QueryAsync<FichaSalidaTipoDto>(
                "dbo.USP_FichaSalidaTipo_ListarActivos",
                commandType: CommandType.StoredProcedure);

            return rows.AsList();
        }

        public async Task<IReadOnlyList<FichaSalidaTipoDetalleDto>> ListarDetallesPorTipoAsync(
            string codFichaSalidaTipo,
            string estado,
            CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@cod_FichaSalidaTipo", codFichaSalidaTipo, DbType.String);
            p.Add("@estado", estado, DbType.String);

            var rows = await con.QueryAsync<FichaSalidaTipoDetalleDto>(
                "dbo.USP_FichaSalidaTipoDetalle_ListarPorTipo",
                p,
                commandType: CommandType.StoredProcedure);

            return rows.AsList();
        }

        public async Task<IReadOnlyList<FichaSalidaHistorialDto>> ListarHistorialAsync(
            string usuario,
            string estadoAutorizacion,
            CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@usuario", usuario, DbType.String);
            p.Add("@estadoAutorizacion", estadoAutorizacion, DbType.String);

            var rows = await con.QueryAsync<FichaSalidaHistorialDto>(
                "dbo.USP_FichaSalida_Historial_PorUsuarioEstado",
                p,
                commandType: CommandType.StoredProcedure);

            return rows.AsList();
        }
    }
}
