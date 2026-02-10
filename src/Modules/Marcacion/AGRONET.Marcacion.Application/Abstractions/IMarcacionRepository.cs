using AGRONET.Marcacion.Application.Contracts;

namespace AGRONET.Marcacion.Application.Abstractions;

public interface IMarcacionRepository
{
    Task<string> RegistrarAsync(RegistrarMarcacionCommand cmd, CancellationToken ct);
}
