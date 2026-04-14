using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public class RegistrarMarcacionResponse
    {
        public bool Success { get; set; }
        public int Codigo { get; set; }
        public string Message { get; set; } = "";
    }
}
