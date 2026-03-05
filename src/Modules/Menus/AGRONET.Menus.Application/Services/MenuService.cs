using AGRONET.Menus.Application.Contracts;
using AGRONET.Menus.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Services
{
    public sealed class MenuService : IMenuService
    {
        private readonly IMenuRepository _repo;

        public MenuService(IMenuRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<MenuDto>> ObtenerArbolPorRolAsync(int idRol, CancellationToken ct)
        {
            var flat = await _repo.ListarPorRolAsync(idRol, ct);
            return BuildTree(flat);
        }

        private static List<MenuDto> BuildTree(List<MenuDto> flat)
        {
            var byId = flat.ToDictionary(x => x.IdMenu);
            var roots = new List<MenuDto>();

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
                    roots.Add(m); // huérfano, lo ponemos como raíz
            }

            SortRecursive(roots);
            return roots;

            static void SortRecursive(List<MenuDto> list)
            {
                list.Sort((a, b) => (a.Orden ?? 0).CompareTo(b.Orden ?? 0));
                foreach (var item in list)
                    SortRecursive(item.Children);
            }
        }

        public Task ReemplazarMenusDeRolAsync(int idRol, List<int> menuIds, string usuario, CancellationToken ct)
    => _repo.ReemplazarMenusDeRolAsync(idRol, menuIds, usuario, ct);
    }
}
