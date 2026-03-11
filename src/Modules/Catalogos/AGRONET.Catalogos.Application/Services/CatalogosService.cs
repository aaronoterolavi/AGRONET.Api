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
    }
}