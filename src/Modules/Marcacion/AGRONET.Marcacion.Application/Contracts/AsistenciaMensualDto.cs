namespace AGRONET.Marcacion.Application.Contracts;

public sealed class AsistenciaMensualDto
{
    public DateTime? FecMarca { get; set; }
    public string? Dni { get; set; }
    public string? Area { get; set; }

    public TimeSpan? Ingreso { get; set; }
    public TimeSpan? Almuerzo { get; set; }
    public TimeSpan? Regreso { get; set; }
    public TimeSpan? Salida { get; set; }

    public string? ObsPapeleta { get; set; }
    public string? Tardanza { get; set; }
}