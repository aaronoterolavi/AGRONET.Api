using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class RegistrarMarcacionCommand
    {
        public string Dni { get; set; } = "";
        public string CodArea { get; set; } = "";
        public string TipoAsistencia { get; set; } = "";
        public string Nid { get; set; } = "";
        public string IdBarra { get; set; } = "";
        public string CodEmpresa { get; set; } = "";

        public string AudUsuarioLogin { get; set; } = "";
        public string AudIpMarca { get; set; } = "";
    }
}
