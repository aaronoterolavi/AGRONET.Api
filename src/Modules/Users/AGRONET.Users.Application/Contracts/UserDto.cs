namespace AGRONET.Users.Application.Contracts
{
    public sealed class UserDto
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string DniNorm { get; set; } = string.Empty;
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public string? Nombres { get; set; }
        public int IdRol { get; set; }
        public string? RolCodigo { get; set; }
        public string? RolNombre { get; set; }
        public string? RolDescripcion { get; set; }
        public string? CodArea { get; set; }
        public string? CodTipoEmpleado { get; set; }
        public string? Flg_sede { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public string? descArea { get; set; }
        public string? CodTrabajador { get; set; }
    }
}