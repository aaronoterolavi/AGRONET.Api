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

        public async Task<(IReadOnlyList<FichaSalidaHistorialDto> Items, int TotalRows)> ListarHistorialAsync(
    string dni,
    string estadoAutorizacion,
    int pageSize,
    int pageNumber,
    CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@usuario", dni, DbType.String);
            p.Add("@estadoAutorizacion", estadoAutorizacion, DbType.String);
            p.Add("@PageSize", pageSize, DbType.Int32);
            p.Add("@PageNumber", pageNumber, DbType.Int32);

            using var multi = await con.QueryMultipleAsync(
                "dbo.USP_FichaSalida_Historial_PorUsuarioEstado",
                p,
                commandType: CommandType.StoredProcedure);

            var items = (await multi.ReadAsync<FichaSalidaHistorialDto>()).AsList();
            var total = await multi.ReadSingleAsync<int>(); // TotalRows (segundo resultset)

            return (items, total);
        }

        public async Task<IReadOnlyList<FichaSalidaEstadoDto>> ListarEstadosAsync(CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var rows = await con.QueryAsync<FichaSalidaEstadoDto>(
                "dbo.USP_ListarFichaSalidaEstado",
                commandType: CommandType.StoredProcedure);

            return rows.AsList();
        }

        private sealed class InsertResultRow
        {
            public int? IdFichaSalida { get; set; }
            public string? MensajeSalida { get; set; }
        }

        public async Task<(int? IdFichaSalida, string MensajeSalida)> InsertarAsync(
            string dni,
            FichaSalidaCrearRequestDto req,
            CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@usuario", dni, DbType.String);
            p.Add("@cod_area", req.CodArea, DbType.String);
            p.Add("@cod_per", req.CodPer, DbType.String);
            p.Add("@cod_tipo_empleado", req.CodTipoEmpleado, DbType.String);
            p.Add("@destino", req.Destino, DbType.String);
            p.Add("@motivo", req.Motivo, DbType.String);
            p.Add("@fechaInicio", req.FechaInicio.ToDateTime(TimeOnly.MinValue), DbType.Date);
            p.Add("@horaInicio", req.HoraInicio, DbType.String);
            p.Add("@fechaFin", req.FechaFin.ToDateTime(TimeOnly.MinValue), DbType.Date);
            p.Add("@horaFin", req.HoraFin, DbType.String);
            p.Add("@cod_destinoDetalle", req.CodDestinoDetalle, DbType.String);

            var row = await con.QuerySingleAsync<InsertResultRow>(
                "dbo.USP_tbl_FichaSalida_insertar",
                p,
                commandType: CommandType.StoredProcedure);

            return (row.IdFichaSalida, row.MensajeSalida ?? "");
        }

    }
}
