using AGRONET.Bienes.Application.DTOs.Bienes;
using AGRONET.Bienes.Application.DTOs.Catalogos;
using AGRONET.Bienes.Application.DTOs.Common;
using AGRONET.Bienes.Application.DTOs.Licencias;

namespace AGRONET.Bienes.Application.Contracts;

public interface IBienesService
{
    // ========================= BIENES CRUD =========================
    Task<PagedResultDto<BienDto>> ListarBienesAsync(BienListarFiltrosDto filtros, CancellationToken ct = default);
    Task<BienDto?> ObtenerBienPorIdAsync(int id, CancellationToken ct = default);
    Task<BienDto?> ObtenerBienPorCodPatrimonialAsync(string codPatrimonial, CancellationToken ct = default);
    Task<OperacionResultadoDto> CrearBienAsync(string dniUsuario, BienCrearRequestDto request, CancellationToken ct = default);
    Task<OperacionResultadoDto> ActualizarBienAsync(string dniUsuario, BienActualizarRequestDto request, CancellationToken ct = default);
    Task<OperacionResultadoDto> EliminarBienAsync(int id, CancellationToken ct = default);

    // ========================= CATÁLOGOS =========================
    Task<IReadOnlyList<TipoBienDto>> ListarTiposBienAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MarcaDto>> ListarMarcasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OficinaDto>> ListarOficinasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProcesadorDto>> ListarProcesadoresAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SoftwareDto>> ListarSoftwareAsync(CancellationToken ct = default);

    // ========================= LICENCIAS =========================
    Task<PagedResultDto<LicenciaDto>> ListarLicenciasAsync(LicenciaListarFiltrosDto filtros, CancellationToken ct = default);
    Task<LicenciaDto?> ObtenerLicenciaPorIdAsync(int id, CancellationToken ct = default);
    Task<OperacionResultadoDto> CrearLicenciaAsync(string dniUsuario, LicenciaCrearRequestDto request, CancellationToken ct = default);
    Task<OperacionResultadoDto> EliminarLicenciaAsync(int id, CancellationToken ct = default);

    // ========================= REPORTES =========================
    Task<IReadOnlyList<LicenciaDto>> ReporteLicenciasPorVencerAsync(int dias, CancellationToken ct = default);
}