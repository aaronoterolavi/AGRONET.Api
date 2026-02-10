using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class RegistrarMarcacionManualRequest
    {
        public string Dni { get; set; } = "";
        public string CodArea { get; set; } = "";
        public DateTime FechMarca { get; set; }
        public string HoraMarca { get; set; } = ""; // HH:mm
        public string TipoAsistencia { get; set; } = "";
        public string? ObsPapeleta { get; set; }
        public string? CodPapeleta { get; set; }
    }
}
