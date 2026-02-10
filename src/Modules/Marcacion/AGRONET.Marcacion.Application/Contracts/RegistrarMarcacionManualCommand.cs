using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class RegistrarMarcacionManualCommand
    {
        public string dni { get; set; } = "";
        public string cod_area { get; set; } = "";
        public DateTime FechMarcaM { get; set; }
        public TimeSpan horaMarcaM { get; set; }
        public string tipoAsistencia { get; set; } = "";
        public string aud_UsuarioLogin { get; set; } = "";
        public string aud_ipMarca { get; set; } = "";
        public string? obsPapeleta { get; set; }
        public string? codPapeleta { get; set; }
    }
}
