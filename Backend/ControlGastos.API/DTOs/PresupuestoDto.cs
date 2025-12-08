namespace ControlGastos.API.DTOs
{
    public class PresupuestoDto
    {
        public int PresupuestoId { get; set; }
        public int TipoGastoId { get; set; }
        public string TipoGastoDescripcion { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal MontoPresupuestado { get; set; }
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
    }

    public class PresupuestoCreateDto
    {
        public int TipoGastoId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal MontoPresupuestado { get; set; }
    }

    public class PresupuestoUpdateDto
    {
        public decimal MontoPresupuestado { get; set; }
    }

    public class PresupuestosPorMesDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public List<PresupuestoItemDto> Items { get; set; } = new List<PresupuestoItemDto>();
    }

    public class PresupuestoItemDto
    {
        public int PresupuestoId { get; set; }
        public int TipoGastoId { get; set; }
        public string TipoGastoDescripcion { get; set; } = string.Empty;
        public decimal MontoPresupuestado { get; set; }
    }
}
