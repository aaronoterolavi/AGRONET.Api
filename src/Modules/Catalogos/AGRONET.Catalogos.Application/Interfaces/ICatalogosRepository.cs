using AGRONET.Catalogos.Application.Contracts;

namespace AGRONET.Catalogos.Application.Interfaces
{
    public interface ICatalogosRepository
    {
        Task<IReadOnlyList<AreaComboDto>> ListarAreasAsync(CancellationToken ct);
    }
}