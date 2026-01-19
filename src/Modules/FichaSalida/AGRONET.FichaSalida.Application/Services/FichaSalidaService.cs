using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using AGRONET.FichaSalida.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Services
{
    public sealed class FichaSalidaService : IFichaSalidaService
    {
        private readonly IFichaSalidaRepository _repo;

        public FichaSalidaService(IFichaSalidaRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<FichaSalidaTipoDto>> ListarTiposAsync(CancellationToken ct = default)
            => _repo.ListarTiposAsync(ct);

        public Task<IReadOnlyList<FichaSalidaTipoDetalleDto>> ListarDetallesPorTipoAsync(
            string codFichaSalidaTipo,
            CancellationToken ct = default)
        {
            codFichaSalidaTipo = (codFichaSalidaTipo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(codFichaSalidaTipo))
                return Task.FromResult<IReadOnlyList<FichaSalidaTipoDetalleDto>>([]);

            // tu SP acepta estado, por defecto "1"
            return _repo.ListarDetallesPorTipoAsync(codFichaSalidaTipo, "1", ct);
        }

        public Task<IReadOnlyList<FichaSalidaHistorialDto>> ListarHistorialPorDniAsync(
   string dni,
   string estadoAutorizacion,
   CancellationToken ct = default)
        {
            dni = (dni ?? "").Trim();
            estadoAutorizacion = (estadoAutorizacion ?? "").Trim();

            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(estadoAutorizacion))
                return Task.FromResult<IReadOnlyList<FichaSalidaHistorialDto>>([]);

            return _repo.ListarHistorialAsync(dni, estadoAutorizacion, ct);
        }
    }
}
