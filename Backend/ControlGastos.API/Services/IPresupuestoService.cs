using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IPresupuestoService
    {
        Task<IEnumerable<PresupuestoDto>> GetAllAsync(int usuarioId, bool esAdmin);
        Task<IEnumerable<PresupuestoDto>> GetByUsuariosAsync(List<int> usuariosIds);
        Task<PresupuestoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<PresupuestosPorMesDto> GetPresupuestosPorMesAsync(int mes, int anio, int usuarioId, bool esAdmin);
        Task<PresupuestosPorMesDto> GetPresupuestosPorMesYUsuariosAsync(int mes, int anio, List<int> usuariosIds);
        Task<PresupuestoDto> CreateAsync(PresupuestoCreateDto dto, int usuarioId);
        Task<PresupuestoDto?> UpdateAsync(int id, PresupuestoUpdateDto dto, int usuarioId, bool esAdmin);
        Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
    }
}
