using AGRONET.Users.Application.Contracts;

namespace AGRONET.Users.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<UserDto>> ListarAsync(bool soloActivos, CancellationToken ct);
        Task<UserDto?> ObtenerPorIdAsync(int idUsuario, CancellationToken ct);
        Task<int> CrearAsync(CreateUserRequestDto request, CancellationToken ct);
        Task ActualizarAsync(int idUsuario, UpdateUserRequestDto request, CancellationToken ct);
        Task CambiarActivoAsync(int idUsuario, bool activo, CancellationToken ct);
    }
}