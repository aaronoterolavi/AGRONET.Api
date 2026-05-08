using AGRONET.Bienes.Application.Contracts;
using AGRONET.Bienes.Application.DTOs.Bienes;
using AGRONET.Bienes.Application.DTOs.Catalogos;
using AGRONET.Bienes.Application.DTOs.Common;
using AGRONET.Bienes.Application.DTOs.Licencias;
using AGRONET.Bienes.Domain.Entities;

namespace AGRONET.Bienes.Application.Services;

public class BienesService : IBienesService
{
    private readonly IBienesRepository _repository;

    public BienesService(IBienesRepository repository)
    {
        _repository = repository;
    }

    // ========================= BIENES =========================

    public async Task<PagedResultDto<BienDto>> ListarBienesAsync(BienListarFiltrosDto filtros, CancellationToken ct = default)
    {
        return await _repository.ListarBienesAsync(filtros, ct);
    }

    public async Task<BienDto?> ObtenerBienPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.ObtenerBienPorIdAsync(id, ct);
    }

    public async Task<BienDto?> ObtenerBienPorCodPatrimonialAsync(string codPatrimonial, CancellationToken ct = default)
    {
        return await _repository.ObtenerBienPorCodPatrimonialAsync(codPatrimonial, ct);
    }

    public async Task<OperacionResultadoDto> CrearBienAsync(string dniUsuario, BienCrearRequestDto request, CancellationToken ct = default)
    {
        var existe = await _repository.ExisteCodPatrimonialAsync(request.cod_patrimonial, null, ct);
        if (existe)
            return OperacionResultadoDto.Error($"El código patrimonial '{request.cod_patrimonial}' ya existe");

        var bien = new Bien
        {
            cod_patrimonial = request.cod_patrimonial,
            txt_nombre = request.txt_nombre,
            txt_descripcion = request.txt_descripcion,
            ide_tipo_bien = request.ide_tipo_bien,
            ide_marca = request.ide_marca,
            txt_modelo = request.txt_modelo,
            txt_serie = request.txt_serie,
            fec_adquisicion = request.fec_adquisicion,
            est_fisico = request.est_fisico,
            cod_area = request.cod_area,
            txt_hostname = request.txt_hostname,
            flg_activo = "SI",
            fec_registro = DateTime.Now
        };

        CaracteristicaTecnica? caracteristica = null;
        if (request.ide_procesador.HasValue || request.num_ram_gb.HasValue || !string.IsNullOrEmpty(request.txt_capac_disco))
        {
            caracteristica = new CaracteristicaTecnica
            {
                ide_procesador = request.ide_procesador,
                num_ram_gb = request.num_ram_gb,
                txt_capac_disco = request.txt_capac_disco,
                flg_disco_solido = request.flg_disco_solido
            };
        }

        var id = await _repository.CrearBienAsync(bien, caracteristica, ct);
        return OperacionResultadoDto.Ok("Bien registrado correctamente", new { ide_bien = id });
    }

    public async Task<OperacionResultadoDto> ActualizarBienAsync(string dniUsuario, BienActualizarRequestDto request, CancellationToken ct = default)
    {
        var existente = await _repository.ObtenerBienPorIdAsync(request.ide_bien, ct);
        if (existente == null)
            return OperacionResultadoDto.Error("El bien no existe");

        var existe = await _repository.ExisteCodPatrimonialAsync(request.cod_patrimonial, request.ide_bien, ct);
        if (existe)
            return OperacionResultadoDto.Error($"El código patrimonial '{request.cod_patrimonial}' ya está en uso");

        var bien = new Bien
        {
            ide_bien = request.ide_bien,
            cod_patrimonial = request.cod_patrimonial,
            txt_nombre = request.txt_nombre,
            txt_descripcion = request.txt_descripcion,
            ide_tipo_bien = request.ide_tipo_bien,
            ide_marca = request.ide_marca,
            txt_modelo = request.txt_modelo,
            txt_serie = request.txt_serie,
            fec_adquisicion = request.fec_adquisicion,
            est_fisico = request.est_fisico,
            cod_area = request.cod_area,
            txt_hostname = request.txt_hostname,
            fec_modificacion = DateTime.Now
        };

        var caracteristica = new CaracteristicaTecnica
        {
            ide_bien = request.ide_bien,
            ide_procesador = request.ide_procesador,
            num_ram_gb = request.num_ram_gb,
            txt_capac_disco = request.txt_capac_disco,
            flg_disco_solido = request.flg_disco_solido
        };

        var resultado = await _repository.ActualizarBienAsync(bien, caracteristica, ct);
        return resultado ? OperacionResultadoDto.Ok("Bien actualizado correctamente") : OperacionResultadoDto.Error("No se pudo actualizar el bien");
    }

    public async Task<OperacionResultadoDto> EliminarBienAsync(int id, CancellationToken ct = default)
    {
        var resultado = await _repository.EliminarBienAsync(id, ct);
        return resultado ? OperacionResultadoDto.Ok("Bien eliminado correctamente") : OperacionResultadoDto.Error("No se pudo eliminar el bien");
    }

    // ========================= CATÁLOGOS =========================

    public async Task<IReadOnlyList<TipoBienDto>> ListarTiposBienAsync(CancellationToken ct = default)
    {
        var tipos = await _repository.ListarTiposBienAsync(ct);
        return tipos.Select(t => new TipoBienDto
        {
            ide_tipo_bien = t.ide_tipo_bien,
            cod_tipo_bien = t.cod_tipo_bien,
            txt_nombre = t.txt_nombre,
            txt_descripcion = t.txt_descripcion
        }).ToList();
    }

    public async Task<IReadOnlyList<MarcaDto>> ListarMarcasAsync(CancellationToken ct = default)
    {
        var marcas = await _repository.ListarMarcasAsync(ct);
        return marcas.Select(m => new MarcaDto
        {
            ide_marca = m.ide_marca,
            cod_marca = m.cod_marca,
            txt_nombre = m.txt_nombre
        }).ToList();
    }

    public async Task<IReadOnlyList<ProcesadorDto>> ListarProcesadoresAsync(CancellationToken ct = default)
    {
        var procesadores = await _repository.ListarProcesadoresAsync(ct);
        return procesadores.Select(p => new ProcesadorDto
        {
            ide_procesador = p.ide_procesador,
            cod_procesador = p.cod_procesador,
            txt_nombre = p.txt_nombre,
            txt_generacion = p.txt_generacion,
            txt_fabricante = p.txt_fabricante
        }).ToList();
    }

    public async Task<IReadOnlyList<SoftwareDto>> ListarSoftwareAsync(CancellationToken ct = default)
    {
        var software = await _repository.ListarSoftwareAsync(ct);
        return software.Select(s => new SoftwareDto
        {
            ide_software = s.ide_software,
            cod_software = s.cod_software,
            txt_nombre = s.txt_nombre,
            txt_version = s.txt_version,
            txt_fabricante = s.txt_fabricante
        }).ToList();
    }

    // ========================= LICENCIAS =========================

    public async Task<PagedResultDto<LicenciaDto>> ListarLicenciasAsync(LicenciaListarFiltrosDto filtros, CancellationToken ct = default)
    {
        return await _repository.ListarLicenciasAsync(filtros, ct);
    }

    public async Task<LicenciaDto?> ObtenerLicenciaPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.ObtenerLicenciaPorIdAsync(id, ct);
    }

    public async Task<OperacionResultadoDto> CrearLicenciaAsync(string dniUsuario, LicenciaCrearRequestDto request, CancellationToken ct = default)
    {
        var licencia = new Licencia
        {
            ide_bien = request.ide_bien,
            ide_software = request.ide_software,
            txt_num_licencia = request.txt_num_licencia,
            fec_instalacion = request.fec_instalacion,
            fec_expiracion = request.fec_expiracion,
            txt_notas = request.txt_notas,
            flg_activo = "SI",
            fec_registro = DateTime.Now
        };

        var id = await _repository.CrearLicenciaAsync(licencia, ct);
        return OperacionResultadoDto.Ok("Licencia registrada correctamente", new { ide_licencia = id });
    }

    public async Task<OperacionResultadoDto> EliminarLicenciaAsync(int id, CancellationToken ct = default)
    {
        var resultado = await _repository.EliminarLicenciaAsync(id, ct);
        return resultado ? OperacionResultadoDto.Ok("Licencia eliminada correctamente") : OperacionResultadoDto.Error("No se pudo eliminar la licencia");
    }

    // ========================= REPORTES =========================

    public async Task<IReadOnlyList<LicenciaDto>> ReporteLicenciasPorVencerAsync(int dias, CancellationToken ct = default)
    {
        return await _repository.ReporteLicenciasPorVencerAsync(dias, ct);
    }
}