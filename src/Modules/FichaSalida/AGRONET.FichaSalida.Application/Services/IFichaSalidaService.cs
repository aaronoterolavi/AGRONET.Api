using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Services
{
    public interface IFichaSalidaService
    {
        Task<IReadOnlyList<FichaSalidaTipoDto>> ListarTiposAsync(CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaTipoDetalleDto>> ListarDetallesPorTipoAsync(
            string codFichaSalidaTipo,
            CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaHistorialDto>> ListarHistorialPorDniAsync(
     string dni,
     string estadoAutorizacion,
     CancellationToken ct = default);
    }
}
