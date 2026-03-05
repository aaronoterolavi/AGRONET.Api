using AGRONET.Menus.Application.Contracts;
using AGRONET.Menus.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Services
{
    public sealed class MenuAdminService : IMenuAdminService
    {
        private readonly IMenuAdminRepository _repo;

        public MenuAdminService(IMenuAdminRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MenuAdminDto>> ListarTodosAsync(bool soloActivos, CancellationToken ct)
            => _repo.ListarTodosAsync(soloActivos, ct);

        public async Task<List<MenuAdminDto>> ListarArbolAsync(bool soloActivos, CancellationToken ct)
        {
            var flat = await _repo.ListarTodosAsync(soloActivos, ct);
            return BuildTree(flat);
        }

        public Task<MenuAdminDto?> ObtenerPorIdAsync(int idMenu, CancellationToken ct)
            => _repo.ObtenerPorIdAsync(idMenu, ct);

        public Task<int> CrearAsync(CreateMenuRequestDto req, string usuario, CancellationToken ct)
            => _repo.CrearAsync(req, usuario, ct);

        public Task ActualizarAsync(int idMenu, UpdateMenuRequestDto req, string usuario, CancellationToken ct)
            => _repo.ActualizarAsync(idMenu, req, usuario, ct);

        public Task CambiarActivoAsync(int idMenu, bool activo, string usuario, CancellationToken ct)
            => _repo.CambiarActivoAsync(idMenu, activo, usuario, ct);

        public Task CambiarOrdenAsync(int idMenu, int orden, string usuario, CancellationToken ct)
            => _repo.CambiarOrdenAsync(idMenu, orden, usuario, ct);

        private static List<MenuAdminDto> BuildTree(List<MenuAdminDto> flat)
        {
            var byId = flat.ToDictionary(x => x.IdMenu);
            var roots = new List<MenuAdminDto>();

            foreach (var m in flat)
            {
                if (m.IdMenuPadre is null || m.IdMenuPadre == 0)
                {
                    roots.Add(m);
                    continue;
                }

                if (byId.TryGetValue(m.IdMenuPadre.Value, out var parent))
                    parent.Children.Add(m);
                else
                    roots.Add(m);
            }

            SortRecursive(roots);
            return roots;

            static void SortRecursive(List<MenuAdminDto> list)
            {
                list.Sort((a, b) => (a.Orden ?? 0).CompareTo(b.Orden ?? 0));
                foreach (var item in list) SortRecursive(item.Children);
            }
        }
    }
}
