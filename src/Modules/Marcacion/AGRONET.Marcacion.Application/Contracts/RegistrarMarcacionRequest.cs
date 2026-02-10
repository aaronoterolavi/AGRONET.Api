using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class RegistrarMarcacionRequest
    {
        public string CodArea { get; set; } = "";
        public string TipoAsistencia { get; set; } = ""; // 2 chars
        public string Nid { get; set; } = "";
        public string IdBarra { get; set; } = "";
        public string CodEmpresa { get; set; } = "";
    }
}
