using AGRONET.Bienes.Application.Contracts;
using AGRONET.Bienes.Application.DTOs.Bienes;
using AGRONET.Bienes.Application.DTOs.Common;
using AGRONET.Bienes.Application.DTOs.Licencias;
using AGRONET.Bienes.Application.DTOs.Mantenimientos;
using AGRONET.Bienes.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AGRONET.Bienes.Infrastructure.Persistence;

public sealed class BienesRepository : IBienesRepository
{
    private readonly string _cs;

    public BienesRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("BD_AGRONET")
              ?? throw new InvalidOperationException("Falta ConnectionStrings:BD_AGRONET");
    }

    // ========================= BIENES =========================

    public async Task<PagedResultDto<BienDto>> ListarBienesAsync(BienListarFiltrosDto filtros, CancellationToken ct)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@cod_patrimonial", filtros.cod_patrimonial);
            p.Add("@txt_nombre", filtros.txt_nombre);
            p.Add("@ide_tipo_bien", filtros.ide_tipo_bien);
            p.Add("@ide_oficina", filtros.ide_oficina);
            p.Add("@est_fisico", filtros.est_fisico);
            p.Add("@cod_area", filtros.cod_area);
            p.Add("@pageSize", filtros.page_size);
            p.Add("@pageNumber", filtros.page_number);
            p.Add("@totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = await cn.QueryAsync<BienDto>(
                "dbo.SGBL_SP_R_BIENES",
                p,
                commandType: CommandType.StoredProcedure
            );

            var totalCount = p.Get<int>("@totalCount");

            return new PagedResultDto<BienDto>
            {
                items = items.ToList(),
                total_count = totalCount,
                page_number = filtros.page_number,
                page_size = filtros.page_size
            };
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error SQL: {ex.Message}");
        }
    }

    public async Task<BienDto?> ObtenerBienPorIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@id", id);

            var result = await cn.QueryFirstOrDefaultAsync<BienDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_BIENES_X_ID",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error SQL: {ex.Message}");
        }
    }


    public async Task<BienDto?> ObtenerBienPorCodPatrimonialAsync(string codPatrimonial, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@cod_patrimonial", codPatrimonial);

            var result = await cn.QueryFirstOrDefaultAsync<BienDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_BIENES_X_CODIGO",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error SQL: {ex.Message}");
        }
    }

    public async Task<int> CrearBienAsync(Bien bien, CaracteristicaTecnica? caracteristica, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            // Usar los nombres EXACTOS que espera el SP
            p.Add("@codPatrimonial", bien.cod_patrimonial);
            p.Add("@nombre", bien.txt_nombre);
            p.Add("@descripcion", bien.txt_descripcion);
            p.Add("@tipoBienId", bien.ide_tipo_bien);
            p.Add("@marcaId", bien.ide_marca);
            p.Add("@oficinaId", bien.ide_oficina);
            p.Add("@modelo", bien.txt_modelo);
            p.Add("@serie", bien.txt_serie);
            p.Add("@fechaAdquisicion", bien.fec_adquisicion);
            p.Add("@estadoFisico", bien.est_fisico);
            p.Add("@codArea", bien.cod_area);
            p.Add("@hostname", bien.txt_hostname);
            p.Add("@procesadorId", caracteristica?.ide_procesador);
            p.Add("@ramGb", caracteristica?.num_ram_gb);
            p.Add("@capacidadDisco", caracteristica?.txt_capac_disco);
            p.Add("@flgDiscoSolido", caracteristica?.flg_disco_solido ?? "NO");
            p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_C_BIEN",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return p.Get<int>("@id");
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al crear bien: {ex.Message}");
        }
    }

    public async Task<bool> ActualizarBienAsync(Bien bien, CaracteristicaTecnica? caracteristica, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@id", bien.ide_bien);
            p.Add("@codPatrimonial", bien.cod_patrimonial);
            p.Add("@nombre", bien.txt_nombre);
            p.Add("@descripcion", bien.txt_descripcion);
            p.Add("@tipoBienId", bien.ide_tipo_bien);
            p.Add("@marcaId", bien.ide_marca);
            p.Add("@oficinaId", bien.ide_oficina);
            p.Add("@modelo", bien.txt_modelo);
            p.Add("@serie", bien.txt_serie);
            p.Add("@fechaAdquisicion", bien.fec_adquisicion);
            p.Add("@estadoFisico", bien.est_fisico);
            p.Add("@codArea", bien.cod_area);
            p.Add("@hostname", bien.txt_hostname);
            p.Add("@procesadorId", caracteristica?.ide_procesador);
            p.Add("@ramGb", caracteristica?.num_ram_gb);
            p.Add("@capacidadDisco", caracteristica?.txt_capac_disco);
            p.Add("@flgDiscoSolido", caracteristica?.flg_disco_solido ?? "NO");
            p.Add("@filasAfectadas", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_U_BIEN",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return p.Get<int>("@filasAfectadas") > 0;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al actualizar bien: {ex.Message}");
        }
    }

    public async Task<bool> EliminarBienAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@id", id);

            var filasAfectadas = await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_D_BIEN",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return filasAfectadas > 0;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al eliminar bien: {ex.Message}");
        }
    }

    public async Task<bool> ExisteCodPatrimonialAsync(string codPatrimonial, int? idExcluir = null, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@codPatrimonial", codPatrimonial);
            p.Add("@idExcluir", idExcluir);
            p.Add("@existe", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_BIENES_EXISTE_CODIGO",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return p.Get<bool>("@existe");
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al verificar código patrimonial: {ex.Message}");
        }
    }

    // ========================= CATÁLOGOS =========================

    public async Task<IReadOnlyList<TipoBien>> ListarTiposBienAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var tipos = await cn.QueryAsync<TipoBien>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_TIPOS_BIEN",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return tipos.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar tipos de bien: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Marca>> ListarMarcasAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var marcas = await cn.QueryAsync<Marca>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_MARCAS",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return marcas.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar marcas: {ex.Message}");
        }
    }
    public async Task<IReadOnlyList<Oficina>> ListarOficinasAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var oficina = await cn.QueryAsync<Oficina>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_OFICINAS",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return oficina.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar oficinas: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Procesador>> ListarProcesadoresAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var procesadores = await cn.QueryAsync<Procesador>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_PROCESADORES",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return procesadores.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar procesadores: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Software>> ListarSoftwareAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var software = await cn.QueryAsync<Software>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_SOFTWARE",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return software.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar software: {ex.Message}");
        }
    }

    // ========================= LICENCIAS =========================

    public async Task<PagedResultDto<LicenciaDto>> ListarLicenciasAsync(LicenciaListarFiltrosDto filtros, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@estadoLicencia", filtros.estado_licencia);
            p.Add("@softwareId", filtros.ide_software);
            p.Add("@diasVencer", filtros.dias_vencer);
            p.Add("@buscar", filtros.buscar);
            p.Add("@pageSize", filtros.page_size);
            p.Add("@pageNumber", filtros.page_number);
            p.Add("@totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = await cn.QueryAsync<LicenciaDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_LICENCIAS",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            var totalCount = p.Get<int>("@totalCount");

            return new PagedResultDto<LicenciaDto>
            {
                items = items.ToList(),
                total_count = totalCount,
                page_number = filtros.page_number,
                page_size = filtros.page_size
            };
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar licencias: {ex.Message}");
        }
    }

    public async Task<LicenciaDto?> ObtenerLicenciaPorIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@id", id);

            var result = await cn.QueryFirstOrDefaultAsync<LicenciaDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_LICENCIAS_X_ID",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al obtener licencia por ID: {ex.Message}");
        }
    }

    public async Task<int> CrearLicenciaAsync(Licencia licencia, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@bienId", licencia.ide_bien);
            p.Add("@softwareId", licencia.ide_software);
            p.Add("@numLicencia", licencia.txt_num_licencia);
            p.Add("@fechaInstalacion", licencia.fec_instalacion);
            p.Add("@fechaExpiracion", licencia.fec_expiracion);
            p.Add("@notas", licencia.txt_notas);
            p.Add("@correo", licencia.txt_correo);
            p.Add("@contrasena", licencia.txt_contrasena);
            p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_C_LICENCIA",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return p.Get<int>("@id");
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al crear licencia: {ex.Message}");
        }
    }

    public async Task<bool> EliminarLicenciaAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@filasAfectadas", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await cn.ExecuteAsync(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_D_LICENCIA",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return p.Get<int>("@filasAfectadas") > 0;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al eliminar licencia: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<LicenciaDto>> ReporteLicenciasPorVencerAsync(int dias, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@dias", dias);

            var licencias = await cn.QueryAsync<LicenciaDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_LICENCIAS_POR_VENCER",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return licencias.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al generar reporte de licencias por vencer: {ex.Message}");
        }
    }

    // ========================= MANTENIMIENTOS =========================

    public async Task<PagedResultDto<MantenimientoDto>> ListarMantenimientosAsync(MantenimientoListarFiltrosDto filtros, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@Start", filtros.page_number * filtros.page_size);
            p.Add("@Length", filtros.page_size);
            p.Add("@Draw", 1);
            p.Add("@ide_bien", filtros.ide_bien);
            p.Add("@ide_tipo_mantenimiento", filtros.ide_tipo_mantenimiento);
            p.Add("@flg_estado", filtros.flg_estado);
            p.Add("@fecha_inicio", filtros.fecha_inicio);
            p.Add("@fecha_fin", filtros.fecha_fin);
            p.Add("@buscar", filtros.buscar);
            p.Add("@cod_area", filtros.cod_area);
            p.Add("@ide_oficina", filtros.ide_oficina);

            var result = await cn.QueryAsync<MantenimientoDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_LISTAR_MANTENIMIENTOS",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            var data = result.ToList();
            var totalCount = data.FirstOrDefault()?.recordsTotal ?? 0;

            // Limpiar las propiedades de paginación de cada item
            foreach (var item in data)
            {
                item.draw = 0;
                item.recordsTotal = 0;
                item.recordsFiltered = 0;
            }

            return new PagedResultDto<MantenimientoDto>
            {
                items = data,
                total_count = totalCount,
                page_number = filtros.page_number,
                page_size = filtros.page_size
            };
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar mantenimientos: {ex.Message}");
        }
    }
    public async Task<MantenimientoDto?> ObtenerMantenimientoPorIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@ide_mantenimiento", id);

            var result = await cn.QueryFirstOrDefaultAsync<MantenimientoDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_OBTENER_MANTENIMIENTO",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al obtener mantenimiento por ID: {ex.Message}");
        }
    }

    public async Task<int> CrearMantenimientoAsync(MantenimientoEquipo mantenimiento, CancellationToken ct = default)
    {
        try
        {
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@ide_mantenimiento", null);
            p.Add("@ide_bien", mantenimiento.ide_bien);
            p.Add("@ide_tipo_mantenimiento", mantenimiento.ide_tipo_mantenimiento);
            p.Add("@flg_estado", mantenimiento.flg_estado);
            p.Add("@fec_mantenimiento", mantenimiento.fec_mantenimiento);
            p.Add("@txt_descripcion", mantenimiento.txt_descripcion);
            p.Add("@txt_tecnico_responsable", mantenimiento.txt_tecnico_responsable);
            p.Add("@txt_observaciones", mantenimiento.txt_observaciones);
            p.Add("@num_costo", mantenimiento.num_costo);
            p.Add("@fec_inicio", mantenimiento.fec_inicio);
            p.Add("@fec_fin", mantenimiento.fec_fin);
            p.Add("@txt_recomendaciones", mantenimiento.txt_recomendaciones);
            p.Add("@flg_garantia", mantenimiento.flg_garantia ?? "N");
            p.Add("@fec_proxima_mantenimiento", mantenimiento.fec_proxima_mantenimiento);
            p.Add("@usu_registro", mantenimiento.usu_registro);

            // Ejecutar y leer el resultado
            var result = await cn.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.SGBL_SP_C_GUARDAR_MANTENIMIENTO",
                p,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 60
            );

            if (result != null)
            {
                return (int)result.ide_mantenimiento;
            }

            return 0;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error SQL al crear mantenimiento: {ex.Message}");
        }
    }

    public async Task<bool> ActualizarMantenimientoAsync(MantenimientoEquipo mantenimiento, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@ide_mantenimiento", mantenimiento.ide_mantenimiento);
            p.Add("@ide_bien", mantenimiento.ide_bien);
            p.Add("@ide_tipo_mantenimiento", mantenimiento.ide_tipo_mantenimiento);
            p.Add("@flg_estado", mantenimiento.flg_estado);
            p.Add("@fec_mantenimiento", mantenimiento.fec_mantenimiento);
            p.Add("@txt_descripcion", mantenimiento.txt_descripcion);
            p.Add("@txt_tecnico_responsable", mantenimiento.txt_tecnico_responsable);
            p.Add("@txt_observaciones", mantenimiento.txt_observaciones);
            p.Add("@num_costo", mantenimiento.num_costo);
            p.Add("@fec_inicio", mantenimiento.fec_inicio);
            p.Add("@fec_fin", mantenimiento.fec_fin);
            p.Add("@txt_recomendaciones", mantenimiento.txt_recomendaciones);
            p.Add("@flg_garantia", mantenimiento.flg_garantia ?? "N");
            p.Add("@fec_proxima_mantenimiento", mantenimiento.fec_proxima_mantenimiento);
            p.Add("@usu_modificacion", mantenimiento.usu_modificacion);

            var result = await cn.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.SGBL_SP_U_MANTENIMIENTO",
                p,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 60
            );

            return result != null && result.codigo == 1;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al actualizar mantenimiento: {ex.Message}");
        }
    }

    public async Task<bool> EliminarMantenimientoAsync(int ide_mantenimiento, string usu_modificacion, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@ide_mantenimiento", ide_mantenimiento);
            p.Add("@usu_modificacion", usu_modificacion);

            // Leer el resultado del SELECT
            var result = await cn.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.SGBL_SP_D_ELIMINAR_MANTENIMIENTO",
                p,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 60
            );

            return result != null && result.codigo == 1;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al eliminar mantenimiento: {ex.Message}");
        }
    }

    public async Task<MantenimientoEstadisticasDto> ObtenerEstadisticasMantenimientoAsync(int? ide_bien = null, CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var p = new DynamicParameters();
            p.Add("@ide_bien", ide_bien);

            var result = await cn.QueryFirstOrDefaultAsync<MantenimientoEstadisticasDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_ESTADISTICAS_MANTENIMIENTO",
                    parameters: p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return result ?? new MantenimientoEstadisticasDto();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al obtener estadísticas de mantenimiento: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<TipoMantenimientoDto>> ListarTiposMantenimientoAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var tipos = await cn.QueryAsync<TipoMantenimientoDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_TIPOS_MANTENIMIENTO",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return tipos.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar tipos de mantenimiento: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<EstadoMantenimientoDto>> ListarEstadosMantenimientoAsync(CancellationToken ct = default)
    {
        try
        {
            await using var cn = new SqlConnection(_cs);
            await cn.OpenAsync(ct);

            var estados = await cn.QueryAsync<EstadoMantenimientoDto>(
                new CommandDefinition(
                    commandText: "dbo.SGBL_SP_R_ESTADOS_MANTENIMIENTO",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                )
            );

            return estados.ToList();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error al listar estados de mantenimiento: {ex.Message}");
        }
    }


}