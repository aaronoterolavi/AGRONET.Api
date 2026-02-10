using AGRONET.Marcacion.Application.Abstractions;
using AGRONET.Marcacion.Application.Contracts;

namespace AGRONET.Marcacion.Application.Services;

public sealed class MarcacionService
{
    private readonly IMarcacionRepository _repo;

    public MarcacionService(IMarcacionRepository repo)
    {
        _repo = repo;
    }

    public async Task<RegistrarMarcacionResponse> RegistrarAsync(RegistrarMarcacionCommand cmd, CancellationToken ct)
    {
        // Validaciones mínimas (rápidas)
        if (string.IsNullOrWhiteSpace(cmd.Dni) || cmd.Dni.Length != 8)
            return new RegistrarMarcacionResponse { Message = "Token inválido (dni)." };

        if (string.IsNullOrWhiteSpace(cmd.AudUsuarioLogin))
            return new RegistrarMarcacionResponse { Message = "Token inválido (username)." };

        if (string.IsNullOrWhiteSpace(cmd.CodArea))
            return new RegistrarMarcacionResponse { Message = "CodArea es requerido." };

        if (string.IsNullOrWhiteSpace(cmd.TipoAsistencia) || cmd.TipoAsistencia.Length != 2)
            return new RegistrarMarcacionResponse { Message = "TipoAsistencia inválido (2 caracteres)." };

        var msg = await _repo.RegistrarAsync(cmd, ct);

        return new RegistrarMarcacionResponse
        {
            Message = string.IsNullOrWhiteSpace(msg) ? "Marcación procesada." : msg
        };
    }
}
