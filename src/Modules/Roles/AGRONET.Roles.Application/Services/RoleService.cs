using AGRONET.Roles.Application.Contracts;
using AGRONET.Roles.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Roles.Application.Services
{
    public sealed class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;

        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public Task<List<RoleDto>> ListarAsync(CancellationToken ct)
            => _repo.ListarAsync(ct);

        public Task<RoleDto?> ObtenerPorIdAsync(int idRol, CancellationToken ct)
            => _repo.ObtenerPorIdAsync(idRol, ct);

        public Task<int> CrearAsync(CreateRoleRequestDto req, CancellationToken ct)
            => _repo.CrearAsync(req, ct);

        public Task ActualizarAsync(int idRol, UpdateRoleRequestDto req, CancellationToken ct)
            => _repo.ActualizarAsync(idRol, req, ct);

        public Task CambiarActivoAsync(int idRol, bool activo, CancellationToken ct)
            => _repo.CambiarActivoAsync(idRol, activo, ct);
    }
}
