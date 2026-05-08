using AGRONET.Catalogos.Application.Contracts;
using AGRONET.Catalogos.Application.Interfaces;

namespace AGRONET.Catalogos.Application.Services
{
    public sealed class CatalogosService : ICatalogosService
    {
        private readonly ICatalogosRepository _repository;

        public CatalogosService(ICatalogosRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<AreaComboDto>> ListarAreasAsync(CancellationToken ct)
            => _repository.ListarAreasAsync(ct);

        // ✅ NUEVO: Listar áreas padre
        public Task<IReadOnlyList<AreaComboDto>> ListarAreasPadreAsync(CancellationToken ct)
            => _repository.ListarAreasPadreAsync(ct);

        // ✅ NUEVO: Listar áreas hijas por código padre
        public Task<IReadOnlyList<AreaComboDto>> ListarAreasHijasAsync(string codPadre, CancellationToken ct)
            => _repository.ListarAreasHijasAsync(codPadre, ct);
    }
}