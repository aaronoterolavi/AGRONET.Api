using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Interfaces
{
    public interface IFichaSalidaRepository
    {
        Task<IReadOnlyList<FichaSalidaTipoDto>> ListarTiposAsync(CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaTipoDetalleDto>> ListarDetallesPorTipoAsync(
            string codFichaSalidaTipo,
            string estado,
            CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaHistorialDto>> ListarHistorialAsync(
            string usuario,
            string estadoAutorizacion,
            CancellationToken ct = default);
    }
}
