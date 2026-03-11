using AGRONET.Users.Application.Contracts;
using AGRONET.Users.Application.Interfaces;

namespace AGRONET.Users.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<UserDto>> ListarAsync(bool soloActivos, CancellationToken ct)
            => _repository.ListarAsync(soloActivos, ct);

        public Task<UserDto?> ObtenerPorIdAsync(int idUsuario, CancellationToken ct)
            => _repository.ObtenerPorIdAsync(idUsuario, ct);

        public Task<int> CrearAsync(CreateUserRequestDto request, CancellationToken ct)
        {
            ValidarCreate(request);
            return _repository.CrearAsync(request, ct);
        }

        public Task ActualizarAsync(int idUsuario, UpdateUserRequestDto request, CancellationToken ct)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("IdUsuario inválido.");

            ValidarUpdate(request);
            return _repository.ActualizarAsync(idUsuario, request, ct);
        }

        public Task CambiarActivoAsync(int idUsuario, bool activo, CancellationToken ct)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("IdUsuario inválido.");

            return _repository.CambiarActivoAsync(idUsuario, activo, ct);
        }

        private static void ValidarCreate(CreateUserRequestDto request)
        {
            if (request is null)
                throw new ArgumentException("Request inválido.");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username es requerido.");

            if (string.IsNullOrWhiteSpace(request.DniNorm))
                throw new ArgumentException("DniNorm es requerido.");

            if (request.IdRol <= 0)
                throw new ArgumentException("IdRol es requerido.");
        }

        private static void ValidarUpdate(UpdateUserRequestDto request)
        {
            if (request is null)
                throw new ArgumentException("Request inválido.");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username es requerido.");

            if (string.IsNullOrWhiteSpace(request.DniNorm))
                throw new ArgumentException("DniNorm es requerido.");

            if (request.IdRol <= 0)
                throw new ArgumentException("IdRol es requerido.");
        }
    }
}