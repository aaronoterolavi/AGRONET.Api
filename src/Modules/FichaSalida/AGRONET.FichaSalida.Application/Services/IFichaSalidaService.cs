using AGRONET.FichaSalida.Application.Contracts.Common;
using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using Microsoft.AspNetCore.Http;
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

        Task<PagedResultDto<FichaSalidaHistorialDto>> ListarHistorialPorDniAsync(
      string dni,
      string estadoAutorizacion,
      int pageSize,
      int pageNumber,
      CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaEstadoDto>> ListarEstadosAsync(CancellationToken ct = default);

        Task<FichaSalidaCrearResponseDto> CrearAsync(
        string dni,
        FichaSalidaCrearRequestDto req,
        IFormFile? documento,
        CancellationToken ct = default);

        Task<OperacionResultadoDto> AnularAsync(int id, CancellationToken ct = default);

        Task<OperacionResultadoDto> ActualizarEstadoAutorizacionAsync(
            int id,
            string estadoAutorizacion,
            CancellationToken ct = default);

        Task<PagedResultDto<FichaSalidaAutorizacionDto>> ListarAsync(
           string codArea,
           string dniAutorizador,
           string codTipoEmpleado,
           FichaSalidaListarAutorizacionesRequestDto req,
           CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaListarPorAreaYFechasDto>> ListarPorAreaYFechasAsync(
           FichaSalidaListarPorAreaYFechasRequestDto request,
           CancellationToken cancellationToken);

        Task<FichaSalidaDetalleDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

    }
}
