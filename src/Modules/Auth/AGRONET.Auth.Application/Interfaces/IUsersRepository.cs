using AGRONET.Auth.Application.Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Interfaces
{
    public interface IUsersRepository
    {
        Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<UserDto?> GetByIdAsync(int idUsuario, CancellationToken ct = default);
        /// <summary>
        /// Inserta usuario en BD_AGRONET desde personalsql.rgpma_trabajador por DNI.
        /// Devuelve (Codigo, Mensaje, NuevoId).
        /// </summary>
        Task<(int Codigo, string Mensaje, int NuevoId)> InsertFromRgpmaByDniAsync(
            string username,
            string dni,
            string? ip,
            string? userAgent,
            CancellationToken ct = default);

        Task UpdateLastLoginAsync(int idUsuario, CancellationToken ct = default);

       

    }
}
