namespace ControlGastos.API.DTOs
{
    public class RegistroGastoDto
    {
        public int RegistroGastoId { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public string FondoMonetarioNombre { get; set; } = string.Empty;
        public string NombreComercio { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public decimal MontoTotal { get; set; }
        public List<RegistroGastoDetalleDto> Detalles { get; set; } = new List<RegistroGastoDetalleDto>();
    }

    public class RegistroGastoDetalleDto
    {
        public int RegistroGastoDetalleId { get; set; }
        public int TipoGastoId { get; set; }
        public string TipoGastoDescripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class RegistroGastoCreateDto
    {
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public string NombreComercio { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public List<RegistroGastoDetalleCreateDto> Detalles { get; set; } = new List<RegistroGastoDetalleCreateDto>();
    }

    public class RegistroGastoDetalleCreateDto
    {
        public int TipoGastoId { get; set; }
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ValidacionPresupuestoDto
    {
        public bool HaySobregiro { get; set; }
        public List<SobregiroDetalleDto> Sobregiros { get; set; } = new List<SobregiroDetalleDto>();
    }

    public class SobregiroDetalleDto
    {
        public int TipoGastoId { get; set; }
        public string TipoGastoDescripcion { get; set; } = string.Empty;
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoEjecutado { get; set; }
        public decimal MontoSobregiro { get; set; }
    }
}
