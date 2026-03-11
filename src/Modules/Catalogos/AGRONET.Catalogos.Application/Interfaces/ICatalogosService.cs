using AGRONET.Catalogos.Application.Contracts;

namespace AGRONET.Catalogos.Application.Interfaces
{
    public interface ICatalogosService
    {
        Task<IReadOnlyList<AreaComboDto>> ListarAreasAsync(CancellationToken ct);
    }
}