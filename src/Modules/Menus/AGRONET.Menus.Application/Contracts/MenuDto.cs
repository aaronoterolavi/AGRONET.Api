using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Contracts
{
    public sealed class MenuDto
    {
        public int IdMenu { get; set; }
        public int? IdMenuPadre { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Ruta { get; set; }
        public int? Orden { get; set; }

        // Árbol
        public List<MenuDto> Children { get; set; } = new();
    }
}
