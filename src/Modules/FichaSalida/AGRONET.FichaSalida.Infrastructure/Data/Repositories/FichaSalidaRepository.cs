using AGRONET.Auth.Infrastructure.Data; // reutilizas tu ISqlConnectionFactory de Auth (o muévelo a Common)
using AGRONET.FichaSalida.Application.Contracts.Common;
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

        public async Task<OperacionResultadoDto> AnularAsync(int id, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@id", id, DbType.Int32);

            return await con.QueryFirstAsync<OperacionResultadoDto>(
                "dbo.USP_tbl_FichaSalida_Anular",
                p,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<OperacionResultadoDto> ActualizarEstadoAutorizacionAsync(
            int id,
            string estadoAutorizacion,
            string observacionesVigilancia,
            CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@id", id, DbType.Int32);
            p.Add("@estadoAutorizacion", estadoAutorizacion, DbType.String);
            p.Add("@observacionesVigilancia", observacionesVigilancia, DbType.String);

            return await con.QueryFirstAsync<OperacionResultadoDto>(
                "dbo.USP_tbl_FichaSalida_ActualizarEstadoAutorizacion",
                p,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PagedResultDto<FichaSalidaAutorizacionDto>> ListarAsync(
           string codArea,
           string documento,
           string codTipoEmpleado,
           string? estadoAutorizacion,
           int pageNumber,
           int pageSize,
           CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@cod_area", codArea, DbType.String);
            p.Add("@documento", documento, DbType.String);
            p.Add("@cod_tipo_empleado", codTipoEmpleado, DbType.String);
            p.Add("@estado_autorizacion", estadoAutorizacion, DbType.String);
            p.Add("@PageNumber", pageNumber, DbType.Int32);
            p.Add("@PageSize", pageSize, DbType.Int32);

            using var multi = await con.QueryMultipleAsync(
                "dbo.USP_FichaSalida_ListarAutorizaciones",
                p,
                commandType: CommandType.StoredProcedure);

            var items = (await multi.ReadAsync<FichaSalidaAutorizacionDto>()).AsList();
            var totalRows = await multi.ReadSingleAsync<int>();

            return new PagedResultDto<FichaSalidaAutorizacionDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRows = totalRows
            };
        }

        public async Task<IReadOnlyList<FichaSalidaListarPorAreaYFechasDto>> ListarPorAreaYFechasAsync(
           FichaSalidaListarPorAreaYFechasRequestDto request,
           CancellationToken cancellationToken)
        {
            using var connection = _factory.CreateBdAgronetConnection();

            var result = await connection.QueryAsync<FichaSalidaListarPorAreaYFechasDto>(
                sql: "dbo.USP_FichaSalida_ListarPorAreaYFechas",
                param: new
                {
                    cod_area = request.CodArea,
                    estadoAutorizacion = request.EstadoAutorizacion,
                    inicio = request.Inicio.Date,
                    fin = request.Fin.Date
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<FichaSalidaDetalleDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@id", id, DbType.Int32);

            return await con.QueryFirstOrDefaultAsync<FichaSalidaDetalleDto>(
                "dbo.USP_tbl_FichaSalida_ObtenerPorId",
                p,
                commandType: CommandType.StoredProcedure);
        }
    }
}
