using AGRONET.Catalogos.Application.Contracts;

namespace AGRONET.Catalogos.Application.Interfaces
{
    public interface ICatalogosRepository
    {
        Task<IReadOnlyList<AreaComboDto>> ListarAreasAsync(CancellationToken ct);
        Task<IReadOnlyList<AreaComboDto>> ListarAreasPadreAsync(CancellationToken ct);

        Task<IReadOnlyList<AreaComboDto>> ListarAreasHijasAsync(string codPadre, CancellationToken ct);
    }
}