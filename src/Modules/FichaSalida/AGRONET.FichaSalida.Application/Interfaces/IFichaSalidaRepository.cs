using AGRONET.FichaSalida.Application.Contracts.Common;
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

        Task<(IReadOnlyList<FichaSalidaHistorialDto> Items, int TotalRows)> ListarHistorialAsync(
     string dni,
     string estadoAutorizacion,
     int pageSize,
     int pageNumber,
     CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaEstadoDto>> ListarEstadosAsync(CancellationToken ct = default);

        Task<FichaSalidaDetalleDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        Task<(int? IdFichaSalida, string MensajeSalida)> InsertarAsync(
        string dni,
        FichaSalidaCrearRequestDto req,
        CancellationToken ct = default);

        Task<OperacionResultadoDto> AnularAsync(int id, CancellationToken ct = default);

        Task<OperacionResultadoDto> ActualizarEstadoAutorizacionAsync(
            int id,
            string estadoAutorizacion,
            string observacionesVigilancia,
            CancellationToken ct = default);

        Task<PagedResultDto<FichaSalidaAutorizacionDto>> ListarAsync(
           string codArea,
           string documento,
           string codTipoEmpleado,
           string? estadoAutorizacion,
           int pageNumber,
           int pageSize,
           CancellationToken ct = default);

        Task<IReadOnlyList<FichaSalidaListarPorAreaYFechasDto>> ListarPorAreaYFechasAsync(
            FichaSalidaListarPorAreaYFechasRequestDto request,
            CancellationToken cancellationToken);
    }



    public interface IFichaSalidaAdjuntoRepository
    {
        Task<long> InsertarAdjuntoAsync(
            int idFichaSalida,
            string fileName,
            string? originalName,
            string? contentType,
            long fileSizeBytes,
            string storageMode,
            string storagePath,
            byte[]? sha256,
            string? createdByDni,
            CancellationToken ct = default);
    }


   
}
