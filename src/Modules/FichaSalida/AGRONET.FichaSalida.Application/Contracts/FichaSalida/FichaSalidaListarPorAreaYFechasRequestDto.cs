namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaListarPorAreaYFechasRequestDto
    {
        public string CodArea { get; set; } = string.Empty;
        public string EstadoAutorizacion { get; set; } = string.Empty;
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
}