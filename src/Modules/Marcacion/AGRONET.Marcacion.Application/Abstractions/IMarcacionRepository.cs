using AGRONET.Marcacion.Application.Contracts;

namespace AGRONET.Marcacion.Application.Abstractions;

public interface IMarcacionRepository
{
    Task<string> RegistrarAsync(RegistrarMarcacionCommand cmd, CancellationToken ct);

    Task<IReadOnlyList<ReporteMarcacionDto>>
       ListarReportePorAreaYRangoAsync(string codArea, DateTime desde, DateTime hasta, CancellationToken ct);

    Task<string> RegistrarManualAsync(RegistrarMarcacionManualCommand cmd, CancellationToken ct);

    Task<IReadOnlyList<TrabajadorDto>>
        ListarTrabajadoresPorAreaAsync(string codArea, CancellationToken ct);
}
