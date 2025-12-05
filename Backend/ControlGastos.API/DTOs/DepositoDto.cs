namespace ControlGastos.API.DTOs
{
    public class DepositoDto
    {
        public int DepositoId { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public string FondoMonetarioNombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class DepositoCreateDto
    {
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class DepositoUpdateDto
    {
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }
}
