using AGRONET.Bienes.Application.DTOs.Bienes;
using AGRONET.Bienes.Application.DTOs.Common;
using AGRONET.Bienes.Application.DTOs.Licencias;
using AGRONET.Bienes.Domain.Entities;
using System.Text.RegularExpressions;

namespace AGRONET.Bienes.Application.Contracts;

public interface IBienesRepository
{
    // ========================= BIENES =========================
    Task<PagedResultDto<BienDto>> ListarBienesAsync(BienListarFiltrosDto filtros, CancellationToken ct = default);
    Task<BienDto?> ObtenerBienPorIdAsync(int id, CancellationToken ct = default);
    Task<BienDto?> ObtenerBienPorCodPatrimonialAsync(string codPatrimonial, CancellationToken ct = default);
    Task<int> CrearBienAsync(Bien bien, CaracteristicaTecnica? caracteristica, CancellationToken ct = default);
    Task<bool> ActualizarBienAsync(Bien bien, CaracteristicaTecnica? caracteristica, CancellationToken ct = default);
    Task<bool> EliminarBienAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteCodPatrimonialAsync(string codPatrimonial, int? idExcluir = null, CancellationToken ct = default);

    // ========================= CATÁLOGOS =========================
    Task<IReadOnlyList<TipoBien>> ListarTiposBienAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Marca>> ListarMarcasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Oficina>> ListarOficinasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Procesador>> ListarProcesadoresAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Software>> ListarSoftwareAsync(CancellationToken ct = default);

    // ========================= LICENCIAS =========================
    Task<PagedResultDto<LicenciaDto>> ListarLicenciasAsync(LicenciaListarFiltrosDto filtros, CancellationToken ct = default);
    Task<LicenciaDto?> ObtenerLicenciaPorIdAsync(int id, CancellationToken ct = default);
    Task<int> CrearLicenciaAsync(Licencia licencia, CancellationToken ct = default);
    Task<bool> EliminarLicenciaAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<LicenciaDto>> ReporteLicenciasPorVencerAsync(int dias, CancellationToken ct = default);
}