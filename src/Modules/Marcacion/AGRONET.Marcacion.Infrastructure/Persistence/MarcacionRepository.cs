using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using AGRONET.Marcacion.Application.Abstractions;
using AGRONET.Marcacion.Application.Contracts;

namespace AGRONET.Marcacion.Infrastructure.Persistence;

public sealed class MarcacionRepository : IMarcacionRepository
{
    private readonly string _cs;

    public MarcacionRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("BD_AGRONET")
              ?? throw new InvalidOperationException("Falta ConnectionStrings:BD_AGRONET");
    }

    public async Task<string> RegistrarAsync(RegistrarMarcacionCommand cmd, CancellationToken ct)
    {
        await using var cn = new SqlConnection(_cs);
        await cn.OpenAsync(ct);

        var p = new DynamicParameters();
        p.Add("@dni", cmd.Dni, DbType.String, size: 8);
        p.Add("@cod_area", cmd.CodArea, DbType.String, size: 8);
        p.Add("@tipoAsistencia", cmd.TipoAsistencia, DbType.String, size: 2);
        p.Add("@Nid", cmd.Nid, DbType.String, size: 30);
        p.Add("@idbarra", cmd.IdBarra, DbType.String, size: 25);
        p.Add("@Cod_empresa", cmd.CodEmpresa, DbType.String, size: 25);
        p.Add("@aud_UsuarioLogin", cmd.AudUsuarioLogin, DbType.String, size: 50);
        p.Add("@aud_ipMarca", cmd.AudIpMarca, DbType.String, size: 25);

        // El SP hace SELECT 'mensaje'
        var msg = await cn.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(
                commandText: "USP_asistencia_insertar",
                parameters: p,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct
            )
        );

        return msg ?? "";
    }


    public async Task<IReadOnlyList<ReporteMarcacionDto>>
        ListarReportePorAreaYRangoAsync(string codArea, DateTime desde, DateTime hasta, CancellationToken ct)
    {
        using var cn = new SqlConnection(_cs);
        var p = new DynamicParameters();
        p.Add("@cod_area", codArea);
        p.Add("@fechDesde", desde);
        p.Add("@fechHasta", hasta);

        return (await cn.QueryAsync<ReporteMarcacionDto>(
            "dbo.USP_asistencia_select_x_area_fecha_rango",
            p, commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<MarcacionSpResult> RegistrarManualAsync(RegistrarMarcacionManualCommand cmd, CancellationToken ct)
    {
        using var cn = new SqlConnection(_cs);
        var p = new DynamicParameters(cmd);
        var result = await cn.QueryFirstOrDefaultAsync<MarcacionSpResult>(
    "dbo.USP_asistencia_insertmanual",
    p,
    commandType: CommandType.StoredProcedure);

        return result ?? new MarcacionSpResult
        {
            Codigo = 500,
            Mensaje = "Sin respuesta del SP"
        };
    }

    public async Task<IReadOnlyList<TrabajadorDto>>
        ListarTrabajadoresPorAreaAsync(string codArea,DateTime fecha, CancellationToken ct)
    {
        using var cn = new SqlConnection(_cs);
        return (await cn.QueryAsync<TrabajadorDto>(
            "dbo.USP_trabajador_listar_x_area",
            new { cod_area = codArea, fecha },
            commandType: CommandType.StoredProcedure)).ToList();
    }
    
    public async Task<List<AperturaMarcacionDto>> 
        ListarAperturasAsync(int? anio, CancellationToken ct)
    {
        using var cn = new SqlConnection(_cs);
        return (await cn.QueryAsync<AperturaMarcacionDto>(
            "dbo.SP_LISTAR_APERTURA_MARCACION",
            new { anio },
            commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<string> RegistrarAperturaAsync(AperturaMarcacionDto entity,CancellationToken ct)
    {
        try
        {
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var parameters = new DynamicParameters();
            parameters.Add("@NUM_ANIO", entity.Anio, DbType.Int32);
            parameters.Add("@NUM_MES", entity.Mes, DbType.Int32);
            parameters.Add("@FEC_INICIO_APERTURA", entity.FechaInicio, DbType.DateTime);
            parameters.Add("@FEC_FIN_APERTURA", entity.FechaFin, DbType.DateTime);
            parameters.Add("@FLG_ACTIVO", entity.Activo, DbType.Boolean);
            parameters.Add("@TXT_OBSERVACION", entity.Observacion, DbType.String, size: 255);
            parameters.Add("@TXT_USU", entity.Usuario, DbType.String, size: 50);

            var msg = await cn.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(
              commandText: "SP_MANTENER_APERTURA_MARCACION",
              parameters: parameters,
              commandType: CommandType.StoredProcedure,
              cancellationToken: ct
          )
      );

            return msg ?? "";
        }
        catch (SqlException ex)
        {
            throw new Exception("Error en base de datos al mantener apertura de marcación.", ex);
        }
    }

}
