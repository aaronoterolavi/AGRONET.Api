using AGRONET.Roles.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Roles.Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> ListarAsync(CancellationToken ct);
        Task<RoleDto?> ObtenerPorIdAsync(int idRol, CancellationToken ct);
        Task<int> CrearAsync(CreateRoleRequestDto req, CancellationToken ct);
        Task ActualizarAsync(int idRol, UpdateRoleRequestDto req, CancellationToken ct);
        Task CambiarActivoAsync(int idRol, bool activo, CancellationToken ct);
    }
}
